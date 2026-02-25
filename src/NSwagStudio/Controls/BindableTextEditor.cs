using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace NSwagStudio.Controls;

/// <summary>A TextEditor wrapper with a bindable Text property and TextMate syntax highlighting.</summary>
public class BindableTextEditor : UserControl
{
    private readonly TextEditor _editor;
    private bool _updatingText;
    private TextMate.Installation? _textMateInstallation;
    private RegistryOptions? _registryOptions;

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<BindableTextEditor, string?>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<string?> SyntaxHighlightingProperty =
        AvaloniaProperty.Register<BindableTextEditor, string?>(nameof(SyntaxHighlighting));

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<BindableTextEditor, bool>(nameof(IsReadOnly));

    public static readonly StyledProperty<bool> ShowLineNumbersProperty =
        AvaloniaProperty.Register<BindableTextEditor, bool>(nameof(ShowLineNumbers), defaultValue: true);

    public BindableTextEditor()
    {
        _editor = new TextEditor
        {
            FontFamily = "Consolas,Menlo,Monaco,monospace",
            FontSize = 13,
            ShowLineNumbers = true,
            Padding = new Thickness(8),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        Content = _editor;

        _editor.TextChanged += OnEditorTextChanged;
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string? SyntaxHighlighting
    {
        get => GetValue(SyntaxHighlightingProperty);
        set => SetValue(SyntaxHighlightingProperty, value);
    }

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public bool ShowLineNumbers
    {
        get => GetValue(ShowLineNumbersProperty);
        set => SetValue(ShowLineNumbersProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty)
        {
            if (!_updatingText)
            {
                _updatingText = true;
                var newText = change.GetNewValue<string?>() ?? string.Empty;
                if (_editor.Document.Text != newText)
                    _editor.Document.Text = newText;
                _updatingText = false;
            }
        }
        else if (change.Property == IsReadOnlyProperty)
        {
            _editor.IsReadOnly = change.GetNewValue<bool>();
        }
        else if (change.Property == ShowLineNumbersProperty)
        {
            _editor.ShowLineNumbers = change.GetNewValue<bool>();
        }
        else if (change.Property == SyntaxHighlightingProperty)
        {
            ApplySyntaxHighlighting(change.GetNewValue<string?>());
        }
    }

    protected override void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _editor.IsReadOnly = IsReadOnly;
        _editor.ShowLineNumbers = ShowLineNumbers;
        ApplySyntaxHighlighting(SyntaxHighlighting);

        if (Application.Current is { } app)
            app.ActualThemeVariantChanged += OnThemeChanged;
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        if (Application.Current is { } app)
            app.ActualThemeVariantChanged -= OnThemeChanged;

        _textMateInstallation?.Dispose();
        _textMateInstallation = null;
        _registryOptions = null;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        // Recreate TextMate with the new theme
        _textMateInstallation?.Dispose();
        _textMateInstallation = null;
        _registryOptions = null;
        ApplySyntaxHighlighting(SyntaxHighlighting);
    }

    private void ApplySyntaxHighlighting(string? language)
    {
        if (string.IsNullOrEmpty(language))
            return;

        try
        {
            if (_textMateInstallation == null)
            {
                var isDark = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
                _registryOptions = new RegistryOptions(isDark ? ThemeName.DarkPlus : ThemeName.LightPlus);
                _textMateInstallation = _editor.InstallTextMate(_registryOptions);
            }

            var langId = language!.ToLowerInvariant() switch
            {
                "javascript" or "json" => "json",
                "c#" or "csharp" => "csharp",
                "typescript" => "typescript",
                "xml" or "xaml" => "xml",
                "yaml" => "yaml",
                _ => language.ToLowerInvariant()
            };

            var scope = _registryOptions!.GetScopeByLanguageId(langId);
            if (scope != null)
                _textMateInstallation.SetGrammar(scope);
        }
        catch
        {
            // Ignore if syntax highlighting fails
        }
    }

    private void OnEditorTextChanged(object? sender, EventArgs e)
    {
        if (!_updatingText)
        {
            _updatingText = true;
            Text = _editor.Document.Text;
            _updatingText = false;
        }
    }
}
