using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using NJsonSchema;
using NSwag;

namespace NSwagStudio.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        NSwagVersionText.Text = $"NSwag Version: {OpenApiDocument.ToolchainVersion}";
        NJsonSchemaVersionText.Text = $"NJsonSchema Version: {JsonSchema.ToolchainVersion}";
    }

    private void OnOpenHyperlink(object? sender, TappedEventArgs e)
    {
        if (sender is Control control && control.Tag is string uri)
        {
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
        }
    }

    private void OnClose(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
