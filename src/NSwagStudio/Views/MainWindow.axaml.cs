using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using NSwagStudio.Helpers;
using NSwagStudio.ViewModels;

namespace NSwagStudio.Views;

public partial class MainWindow : Window
{
    private readonly string[]? _startupArgs;

    public MainWindow() : this(null) { }

    public MainWindow(string[]? args)
    {
        _startupArgs = args;
        InitializeComponent();
        DialogService.MainWindow = this;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var inWindowMenu = this.FindControl<Menu>("InWindowMenu");
            if (inWindowMenu != null)
                inWindowMenu.IsVisible = false;

            var nativeMenu = NativeMenu.GetMenu(this);
            if (nativeMenu != null)
            {
                foreach (var topItem in nativeMenu.Items.OfType<NativeMenuItem>())
                {
                    if (topItem.Menu == null) continue;
                    foreach (var item in topItem.Menu.Items.OfType<NativeMenuItem>())
                    {
                        if (item.Header == "About NSwagStudio")
                            item.Click += (_, _) => OnShowAbout(null, null!);
                        else if (item.Header == "Documentation")
                            item.Click += (_, _) => OnOpenDocumentation(null, null!);
                    }
                }
            }
        }

        Opened += OnWindowOpened;
        Closing += OnWindowClosing;
        Closed += OnWindowClosed;

        LoadWindowState();
    }

    private MainWindowModel Model => (MainWindowModel)Resources["ViewModel"]!;

    private async void OnWindowOpened(object? sender, EventArgs e)
    {
        await Model.LoadApplicationSettingsAsync(_startupArgs);
    }

    private void LoadWindowState()
    {
        Width = ApplicationSettings.GetSetting("WindowWidth", Width);
        Height = ApplicationSettings.GetSetting("WindowHeight", Height);

        var left = ApplicationSettings.GetSetting("WindowLeft", double.NaN);
        var top = ApplicationSettings.GetSetting("WindowTop", double.NaN);
        if (!double.IsNaN(left) && !double.IsNaN(top))
        {
            Position = new Avalonia.PixelPoint((int)left, (int)top);
            WindowStartupLocation = WindowStartupLocation.Manual;
        }
    }

    private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;

        foreach (var document in Model.Documents.ToArray())
        {
            var success = await Model.CloseDocumentAsync(document);
            if (!success)
                return;
        }

        Model.SaveWindowDocuments();
        Model.CallOnUnloaded();
        Model.Documents.Clear();

        Closing -= OnWindowClosing;
        Close();
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        ApplicationSettings.SetSetting("WindowWidth", Width);
        ApplicationSettings.SetSetting("WindowHeight", Height);
        ApplicationSettings.SetSetting("WindowLeft", (double)Position.X);
        ApplicationSettings.SetSetting("WindowTop", (double)Position.Y);
    }

    private void OnOpenDocumentation(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/RicoSuter/NSwag/wiki/NSwagStudio")
        {
            UseShellExecute = true
        });
    }

    private void OnShowAbout(object? sender, RoutedEventArgs e)
    {
        var about = new AboutWindow();
        about.ShowDialog(this);
    }

    private void OnTabHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.IsMiddleButtonPressed &&
            sender is Control { DataContext: DocumentModel document })
        {
            _ = Model.CloseDocumentAsync(document);
            e.Handled = true;
        }
    }
}
