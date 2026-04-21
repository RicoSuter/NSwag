using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NSwagStudio.Helpers;

namespace NSwagStudio.Controls;

public partial class FileOpenPicker : UserControl
{
    public static readonly StyledProperty<string?> FilePathProperty =
        AvaloniaProperty.Register<FileOpenPicker, string?>(nameof(FilePath),
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<string?> DefaultExtensionProperty =
        AvaloniaProperty.Register<FileOpenPicker, string?>(nameof(DefaultExtension));

    public static readonly StyledProperty<string?> FilterProperty =
        AvaloniaProperty.Register<FileOpenPicker, string?>(nameof(Filter));

    public FileOpenPicker()
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

        var results = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select File",
            AllowMultiple = false,
            FileTypeFilter = ParseFilter(Filter)
        });

        if (results.Count > 0)
            FilePath = results[0].Path.LocalPath;
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
