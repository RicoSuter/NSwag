using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            // TODO: Enable this
            //var length = ApplicationSettings.GetSetting("WindowSplitter", (double)0); 
            //if (length != 0)
            //    Grid.ColumnDefinitions[0].Width = new GridLength(length, GridUnitType.Pixel);

            Width = ApplicationSettings.GetSetting("WindowWidth", Width);
            Height = ApplicationSettings.GetSetting("WindowHeight", Height);
            Left = ApplicationSettings.GetSetting("WindowLeft", Left);
            Top = ApplicationSettings.GetSetting("WindowTop", Top);
            WindowState = ApplicationSettings.GetSetting("WindowState", WindowState);

            if (Left == double.NaN)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private bool _cancelled = false;
         
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_cancelled)
            {
                Model.CallOnUnloaded();
                Model.Documents.Clear();

                e.Cancel = true;
                _cancelled = true;

                Dispatcher.InvokeAsync(() => { Close(); });
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {

            // TODO: Enable this
            //if (Grid.ColumnDefinitions[0].Width.IsAbsolute)
            //    ApplicationSettings.SetSetting("WindowSplitter", Grid.ColumnDefinitions[0].Width.Value);

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
