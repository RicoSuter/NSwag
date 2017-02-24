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
            Loaded += delegate { UpdateCodeGeneratorTabs(); };
            Unloaded += OnUnloaded;
        }

        private DocumentViewModel Model => (DocumentViewModel)Resources["ViewModel"];

        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document", typeof(DocumentModel), typeof(DocumentView), new PropertyMetadata(default(DocumentModel), OnDocumentChanged));

        private static void OnDocumentChanged(DependencyObject view, DependencyPropertyChangedEventArgs args)
        {
            var documentView = (DocumentView)view;
            var vm = documentView.Model;
            if (vm.Document != args.NewValue)
            {
                if (vm.Document != null)
                    vm.Document.PropertyChanged -= documentView.DocumentOnPropertyChanged;

                vm.Document = (DocumentModel)args.NewValue;

                if (vm.Document != null)
                    vm.Document.PropertyChanged += documentView.DocumentOnPropertyChanged;

                documentView.UpdateCodeGeneratorTabs();
            }
        }

        private void DocumentOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(DocumentModel.SelectedCodeGenerators))
                UpdateCodeGeneratorTabs();
        }

        private void UpdateCodeGeneratorTabs()
        {
            if (OutputTabs.SelectedIndex != -1)
            {
                var selectedCodeGenerator = Model.Document.CodeGenerators.ToList()[OutputTabs.SelectedIndex];
                if (selectedCodeGenerator.View.IsSelected == false)
                    OutputTabs.SelectedIndex = 0;
            }

            foreach (var item in OutputTabs.Items.OfType<CodeGeneratorModel>())
            {
                var container = OutputTabs.ItemContainerGenerator.ContainerFromItem(item) as TabItem;
                if (container != null)
                    container.Visibility = item.View.IsSelected ? Visibility.Visible : Visibility.Collapsed;
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
