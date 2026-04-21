using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NSwagStudio.Helpers;

namespace NSwagStudio.Controls;

public partial class FileSavePicker : UserControl
{
    public static readonly StyledProperty<string?> FilePathProperty =
        AvaloniaProperty.Register<FileSavePicker, string?>(nameof(FilePath),
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<string?> DefaultExtensionProperty =
        AvaloniaProperty.Register<FileSavePicker, string?>(nameof(DefaultExtension));

    public static readonly StyledProperty<string?> FilterProperty =
        AvaloniaProperty.Register<FileSavePicker, string?>(nameof(Filter));

    public FileSavePicker()
    {
        InitializeComponent();
    }

    public string? FilePath
    {
        get => GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    public string? DefaultExtension
    {
        get => GetValue(DefaultExtensionProperty);
        set => SetValue(DefaultExtensionProperty, value);
    }

    public string? Filter
    {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    private async void OnBrowseClick(object? sender, RoutedEventArgs e)
    {
        var window = DialogService.MainWindow;
        if (window == null)
            return;

        var result = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Select File",
            SuggestedFileName = System.IO.Path.GetFileName(FilePath ?? ""),
            FileTypeChoices = ParseFilter(Filter)
        });

        if (result != null)
            FilePath = result.Path.LocalPath;
    }

    private static List<FilePickerFileType> ParseFilter(string? filter)
    {
        var types = new List<FilePickerFileType>();
        if (string.IsNullOrEmpty(filter))
            return types;

        var parts = filter!.Split('|');
        for (int i = 0; i + 1 < parts.Length; i += 2)
        {
            types.Add(new FilePickerFileType(parts[i].Trim())
            {
                Patterns = parts[i + 1].Split(';').Select(p => p.Trim()).ToList()
            });
        }
        return types;
    }
}
