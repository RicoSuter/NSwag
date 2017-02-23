using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyToolkit.Mvvm;
using NSwagStudio.ViewModels;
using ViewModelBase = MyToolkit.Mvvm.ViewModelBase;

namespace NSwagStudio.Views
{
    public partial class DocumentView
    {
        public DocumentView()
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Unloaded += OnUnloaded;
        }
        
        private DocumentViewModel Model => (DocumentViewModel)Resources["ViewModel"];

        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document", typeof(DocumentModel), typeof(DocumentView), new PropertyMetadata(default(DocumentModel), OnDocumentChanged));

        private static void OnDocumentChanged(DependencyObject view, DependencyPropertyChangedEventArgs args)
        {
            var documentView = (DocumentView) view; 
            var vm = documentView.Model;
            if (vm.Document != args.NewValue)
            {
                if (vm.Document != null)
                    vm.Document.PropertyChanged -= documentView.DocumentOnPropertyChanged;

                vm.Document = (DocumentModel)args.NewValue;

                if (vm.Document != null)
                    vm.Document.PropertyChanged += documentView.DocumentOnPropertyChanged;
            }
        }

        private void DocumentOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(DocumentModel.SelectedCodeGenerators))
            {
                var selectedCodeGenerator = Model.Document.CodeGenerators.ToList()[OutputTabs.SelectedIndex]; 
                if (selectedCodeGenerator.View.IsActive == false)
                {
                    OutputTabs.SelectedIndex = 0; 
                }
            }
        }

        public DocumentModel Document
        {
            get { return (DocumentModel)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            App.Telemetry.TrackEvent("Generate", new Dictionary<string, string>
            {
                { "Generator", Model.Document.GetSwaggerGeneratorView().Title }
            });
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (var generatorView in Model.Document.CodeGenerators
                .Where(c => c.View is UserControl)
                .Select(c => c.View)
                .OfType<UserControl>()
                .Concat(Model.Document.SwaggerGeneratorViews.OfType<UserControl>()))
            {
                var vm = generatorView.Resources["ViewModel"] as ViewModelBase;
                vm?.CallOnUnloaded();
            }
        }
    }
}
