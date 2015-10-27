using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using MyToolkit.Storage;
using MyToolkit.Utilities;

namespace NSwagStudio.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckForApplicationUpdate();
            LoadWindowState();
        }

        private async void CheckForApplicationUpdate()
        {
            var updater = new ApplicationUpdater(
                "NSwagStudio.msi",
                GetType().Assembly,
                "http://rsuter.com/Projects/NSwagStudio/updates.php");

            await updater.CheckForUpdate(this);
        }

        private void LoadWindowState()
        {
            var length = ApplicationSettings.GetSetting("WindowSplitter", (double)0); 
            if (length != 0)
                Grid.ColumnDefinitions[0].Width = new GridLength(length, GridUnitType.Pixel);

            Width = ApplicationSettings.GetSetting("WindowWidth", Width);
            Height = ApplicationSettings.GetSetting("WindowHeight", Height);
            Left = ApplicationSettings.GetSetting("WindowLeft", Left);
            Top = ApplicationSettings.GetSetting("WindowTop", Top);
            WindowState = ApplicationSettings.GetSetting("WindowState", WindowState);

            if (Left == double.NaN)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (Grid.ColumnDefinitions[0].Width.IsAbsolute)
                ApplicationSettings.SetSetting("WindowSplitter", Grid.ColumnDefinitions[0].Width.Value);

            ApplicationSettings.SetSetting("WindowWidth", Width);
            ApplicationSettings.SetSetting("WindowHeight", Height);
            ApplicationSettings.SetSetting("WindowLeft", Left);
            ApplicationSettings.SetSetting("WindowTop", Top);
            ApplicationSettings.SetSetting("WindowState", WindowState);
        }

        private void OnOpenHyperlink(object sender, RoutedEventArgs e)
        {
            var uri = ((Hyperlink)sender).NavigateUri;
            Process.Start(uri.ToString());
        }
    }
}
