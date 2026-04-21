using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using NSwagStudio.ViewModels;

namespace NSwagStudio.Views;

public partial class DocumentView : UserControl
{
    public DocumentView()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            UpdateInputTabs();
            UpdateOutputTabs();
        };
        Unloaded += OnUnloaded;
    }

    private DocumentViewModel Model => (DocumentViewModel)Resources["ViewModel"]!;

    public static readonly StyledProperty<DocumentModel?> DocumentProperty =
        AvaloniaProperty.Register<DocumentView, DocumentModel?>(nameof(Document));

    public DocumentModel? Document
    {
        get => GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == DocumentProperty)
        {
            var oldDoc = change.GetOldValue<DocumentModel?>();
            var newDoc = change.GetNewValue<DocumentModel?>();

            if (oldDoc != null)
                oldDoc.PropertyChanged -= DocumentOnPropertyChanged;

            Model.Document = newDoc;

            if (newDoc != null)
                newDoc.PropertyChanged += DocumentOnPropertyChanged;

            UpdateInputTabs();
            UpdateOutputTabs();
        }
    }

    private void DocumentOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DocumentModel.SelectedCodeGenerators))
            UpdateOutputTabs();
    }

    private void UpdateInputTabs()
    {
        var inputTabControl = this.FindControl<TabControl>("InputTabControl");
        if (inputTabControl == null || Model.Document == null)
            return;

        // Detach views from old TabItems before clearing to allow re-parenting
        foreach (var item in inputTabControl.Items.OfType<TabItem>())
            item.Content = null;

        inputTabControl.Items.Clear();
        foreach (var view in Model.Document.SwaggerGeneratorViews)
        {
            inputTabControl.Items.Add(new TabItem
            {
                Header = view.Title,
                Content = view as Control
            });
        }

        // Select tab matching current swagger generator
        try
        {
            var selectedView = Model.Document.GetSwaggerGeneratorView();
            var index = Array.IndexOf(Model.Document.SwaggerGeneratorViews, selectedView);
            if (index >= 0)
                inputTabControl.SelectedIndex = index;
        }
        catch
        {
            if (inputTabControl.Items.Count > 0)
                inputTabControl.SelectedIndex = 0;
        }
    }

    private void UpdateOutputTabs()
    {
        if (Model.Document == null)
            return;

        var outputTabs = this.FindControl<TabControl>("OutputTabs");
        if (outputTabs == null)
            return;

        // Remember which generator was selected
        var previousGen = outputTabs.SelectedItem is TabItem prevTab ? prevTab.Tag : null;

        // Detach views from old TabItems before clearing to allow re-parenting
        foreach (var item in outputTabs.Items.OfType<TabItem>())
            item.Content = null;

        outputTabs.Items.Clear();
        var selectIndex = 0;
        var i = 0;
        foreach (var gen in Model.Document.SelectedCodeGenerators)
        {
            var tabItem = new TabItem
            {
                Header = gen.View.Title,
                Content = gen.View,
                Tag = gen
            };
            outputTabs.Items.Add(tabItem);
            if (gen == previousGen)
                selectIndex = i;
            i++;
        }

        if (outputTabs.Items.Count > 0)
            outputTabs.SelectedIndex = Math.Min(selectIndex, outputTabs.Items.Count - 1);
    }

    private void OnInputTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (Model.Document == null)
            return;

        var tabControl = sender as TabControl;
        if (tabControl?.SelectedItem is TabItem tabItem && tabItem.Content is ISwaggerGeneratorView view)
        {
            Model.Document.Document.SelectedSwaggerGenerator = view.Command;
        }
    }

    private void OnUnloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Model.Document == null)
            return;

        foreach (var generatorView in Model.Document.CodeGenerators
            .Where(c => c.View is UserControl)
            .Select(c => c.View)
            .OfType<UserControl>()
            .Concat(Model.Document.SwaggerGeneratorViews.OfType<UserControl>()))
        {
            var vm = generatorView.Resources["ViewModel"] as ViewModelBase;
            vm?.CallOnUnloaded();
        }
    }
}
