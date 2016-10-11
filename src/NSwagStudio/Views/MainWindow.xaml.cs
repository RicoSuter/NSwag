using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using MyToolkit.Mvvm;
using MyToolkit.Storage;
using MyToolkit.UI;
using MyToolkit.Utilities;
using NSwagStudio.ViewModels;
using Newtonsoft.Json;

namespace NSwagStudio.Views
{
    public partial class MainWindow : Window
    {
        private bool _closeCancelled = false;

        public MainWindow()
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            RegisterShortcuts();
            CheckForApplicationUpdate();
            LoadWindowState();
            RegisterFileOpenHandler();

            Title += IntPtr.Size == 4 ? " (x86)" : " (x64)";
        }

        private MainWindowModel Model => (MainWindowModel)Resources["ViewModel"];

        private void RegisterShortcuts()
        {
            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.N, ModifierKeys.Control),
                () => Model.CreateDocumentCommand.TryExecute());
            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.O, ModifierKeys.Control),
                () => Model.OpenDocumentCommand.TryExecute());
            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.S, ModifierKeys.Control),
                () => Model.SaveDocumentCommand.TryExecute(Model.SelectedDocument));
            ShortcutManager.RegisterShortcut(typeof(MainWindow), new KeyGesture(Key.W, ModifierKeys.Control),
                () => Model.CloseDocumentCommand.TryExecute(Model.SelectedDocument));
        }

        private void RegisterFileOpenHandler()
        {
            var fileHandler = new FileOpenHandler();
            fileHandler.FileOpen += async (sender, args) => { await Model.OpenDocumentAsync(args.FileName); };
            fileHandler.Initialize(this);
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
            Width = ApplicationSettings.GetSetting("WindowWidth", Width);
            Height = ApplicationSettings.GetSetting("WindowHeight", Height);
            Left = ApplicationSettings.GetSetting("WindowLeft", Left);
            Top = ApplicationSettings.GetSetting("WindowTop", Top);
            WindowState = ApplicationSettings.GetSetting("WindowState", WindowState);

            if (Left == double.NaN)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        protected override async void OnClosing(CancelEventArgs e)
        {
            if (!_closeCancelled)
            {
                e.Cancel = true;

                var paths = Model.Documents
                    .Where(d => System.IO.File.Exists(d.Path))
                    .Select(d => d.Path)
                    .ToArray();

                foreach (var document in Model.Documents.ToArray())
                {
                    var success = await Model.CloseDocumentAsync(document);
                    if (!success)
                    {
                        base.OnClosing(e);
                        return;
                    }
                }

                ApplicationSettings.SetSetting("NSwagSettings", JsonConvert.SerializeObject(paths, Formatting.Indented));

                Model.CallOnUnloaded();
                Model.Documents.Clear();

                _closeCancelled = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Dispatcher.InvokeAsync(Close);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
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
