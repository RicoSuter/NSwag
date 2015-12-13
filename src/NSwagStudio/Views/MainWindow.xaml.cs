using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MyToolkit.Mvvm;
using MyToolkit.Storage;
using MyToolkit.Utilities;
using NSwagStudio.ViewModels;
using ViewModelBase = NSwagStudio.ViewModels.ViewModelBase;

namespace NSwagStudio.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            CheckForApplicationUpdate();
            LoadWindowState();
        }

        private MainWindowModel Model { get { return (MainWindowModel)Resources["ViewModel"]; } }

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

            foreach (var generatorView in Model.ClientGenerators.OfType<UserControl>()
                .Concat(Model.SwaggerGenerators.OfType<UserControl>()))
            {
                var vm = generatorView.Resources["ViewModel"] as ViewModelBase; 
                if (vm != null)
                    vm.CallOnUnloaded();
            }
        }

        private void OnOpenHyperlink(object sender, RoutedEventArgs e)
        {
            var uri = ((Hyperlink)sender).NavigateUri;
            Process.Start(uri.ToString());
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            App.Telemetry.TrackEvent("Generate", new Dictionary<string, string>
            {
                { "Generator", Model.SelectedSwaggerGenerator.Title }
            });
        }
    }
}
