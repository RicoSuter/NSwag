using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyToolkit.Mvvm;
using NSwag.CodeGeneration;
using NSwagStudio.ViewModels;
using ViewModelBase = NSwagStudio.ViewModels.ViewModelBase;

namespace NSwagStudio.Views
{
    public partial class DocumentView : UserControl
    {
        public DocumentView()
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Unloaded += OnUnloaded;
        }

        private DocumentViewModel Model { get { return (DocumentViewModel)Resources["ViewModel"]; } }

        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document", typeof (NSwagDocument), typeof (DocumentView), new PropertyMetadata(default(NSwagDocument), OnDocumentChanged));

        private static void OnDocumentChanged(DependencyObject view, DependencyPropertyChangedEventArgs args)
        {
            var vm = ((DocumentView) view).Model;
            if (vm.Document != args.NewValue)
                vm.Document = (NSwagDocument) args.NewValue;
        }

        public NSwagDocument Document
        {
            get { return (NSwagDocument) GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            App.Telemetry.TrackEvent("Generate", new Dictionary<string, string>
            {
                { "Generator", Model.SwaggerGenerators[Document.SelectedSwaggerGenerator].Title }
            });
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (var generatorView in Model.CodeGenerators.OfType<UserControl>()
                .Concat(Model.SwaggerGenerators.OfType<UserControl>()))
            {
                var vm = generatorView.Resources["ViewModel"] as ViewModelBase;
                if (vm != null)
                    vm.CallOnUnloaded();
            }
        }
    }
}
