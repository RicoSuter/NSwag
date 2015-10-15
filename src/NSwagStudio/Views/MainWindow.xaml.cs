using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using MyToolkit.Utilities;

namespace NSwagStudio.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckForApplicationUpdate();
        }

        private async void CheckForApplicationUpdate()
        {
            var updater = new ApplicationUpdater(
                "NSwagStudio.msi",
                GetType().Assembly,
                "http://rsuter.com/Projects/NSwagStudio/updates.php");

            await updater.CheckForUpdate(this);
        }

        private void OnOpenHyperlink(object sender, RoutedEventArgs e)
        {
            var uri = ((Hyperlink)sender).NavigateUri;
            Process.Start(uri.ToString());
        }
    }
}
