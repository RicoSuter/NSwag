using System.Windows;
using System.Windows.Controls;

namespace NSwagStudio.Controls
{
    public abstract partial class FilePickerBase : UserControl
    {
        // TODO: Move to MyToolkit

        protected FilePickerBase()
        {
            InitializeComponent();
            ((FrameworkElement)Content).DataContext = this;
        }

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
            "FilePath", typeof(string), typeof(FilePickerBase), new PropertyMetadata(default(string)));

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public static readonly DependencyProperty DefaultExtensionProperty = DependencyProperty.Register(
            "DefaultExtension", typeof(string), typeof(FilePickerBase), new PropertyMetadata(".*"));

        public string DefaultExtension
        {
            get { return (string)GetValue(DefaultExtensionProperty); }
            set { SetValue(DefaultExtensionProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            "Filter", typeof(string), typeof(FilePickerBase), new PropertyMetadata("All Files (.*)|*.*"));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        protected abstract void SelectFile();

        private void OnSelectFile(object sender, RoutedEventArgs e)
        {
            SelectFile();
        }
    }
}
