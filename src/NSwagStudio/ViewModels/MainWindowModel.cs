using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using NSwag.Commands;
using NSwagStudio.Helpers;

namespace NSwagStudio.ViewModels;

/// <summary>The view model for the MainWindow.</summary>
public class MainWindowModel : ViewModelBase
{
    private DocumentModel? _selectedDocument;

    /// <summary>Initializes a new instance of the <see cref="MainWindowModel"/> class.</summary>
    public MainWindowModel()
    {
        CreateDocumentCommand = new RelayCommand(CreateDocument);
        OpenDocumentCommand = new AsyncRelayCommand(OpenDocumentAsync);

        CloseDocumentCommand = new AsyncRelayCommand<DocumentModel?>(async document => { if (document != null) await CloseDocumentAsync(document); }, document => document != null);
        CloseAllDocumentsCommand = new AsyncRelayCommand(async () => await CloseAllDocumentsAsync(Documents!));
        SaveDocumentCommand = new AsyncRelayCommand<DocumentModel?>(async document => { if (document != null) await SaveDocumentAsync(document); }, document => document != null);
        SaveAsDocumentCommand = new AsyncRelayCommand<DocumentModel?>(async document => { if (document != null) await SaveAsDocumentAsync(document); }, document => document != null);
        SaveAllDocumentsCommand = new AsyncRelayCommand(async () => await SaveAllDocumentAsync(Documents!));

        Documents = new ObservableCollection<DocumentModel>();
    }

    public ObservableCollection<DocumentModel> Documents { get; private set; }

    /// <summary>Gets or sets the selected document. </summary>
    public DocumentModel? SelectedDocument
    {
        get => _selectedDocument;
        set
        {
            if (Set(ref _selectedDocument, value))
            {
                CloseDocumentCommand.NotifyCanExecuteChanged();
                SaveDocumentCommand.NotifyCanExecuteChanged();
                SaveAsDocumentCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public RelayCommand CreateDocumentCommand { get; }

    public AsyncRelayCommand OpenDocumentCommand { get; }

    public AsyncRelayCommand<DocumentModel?> CloseDocumentCommand { get; }

    public AsyncRelayCommand CloseAllDocumentsCommand { get; }

    public AsyncRelayCommand<DocumentModel?> SaveDocumentCommand { get; }

    public AsyncRelayCommand<DocumentModel?> SaveAsDocumentCommand { get; }

    public AsyncRelayCommand SaveAllDocumentsCommand { get; }

    public string NSwagVersion => OpenApiDocument.ToolchainVersion;

    public string NJsonSchemaVersion => JsonSchema.ToolchainVersion;

    public async Task LoadApplicationSettingsAsync()
    {
        try
        {
            await Task.Delay(500);
            var settings = ApplicationSettings.GetSetting("NSwagSettings", string.Empty);
            if (settings != string.Empty)
            {
                var paths = JsonConvert.DeserializeObject<string[]>(settings)?
                    .Where(File.Exists)
                    .ToArray() ?? Array.Empty<string>();

                if (paths.Length > 0)
                {
                    foreach (var path in paths)
                        await OpenDocumentAsync(path);

                    if (Documents.Any())
                        SelectedDocument = Documents.Last();
                    else
                        CreateDocument();
                }
                else if (!Documents.Any())
                    CreateDocument();
            }
            else if (!Documents.Any())
                CreateDocument();
        }
        catch
        {
            if (!Documents.Any())
                CreateDocument();
        }

        if (Documents.Any())
            SelectedDocument = Documents.First();
    }

    private void CreateDocument()
    {
        var document = NSwagDocument.Create();
        var documentModel = new DocumentModel(document);
        Documents.Add(documentModel);
        SelectedDocument = documentModel;
    }

    private async Task OpenDocumentAsync()
    {
        var window = DialogService.MainWindow;
        if (window == null)
            return;

        var results = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open NSwag settings file",
            AllowMultiple = true,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("NSwag file") { Patterns = new[] { "*.nswag", "*nswag.json" } }
            }
        });

        foreach (var file in results)
        {
            await OpenDocumentAsync(file.Path.LocalPath);
        }
    }

    public async Task OpenDocumentAsync(string filePath)
    {
        await RunTaskAsync(async () =>
        {
            var currentDocument = Documents.SingleOrDefault(d => d.Document.Path == filePath);
            if (currentDocument != null)
                SelectedDocument = currentDocument;
            else
            {
                var document = new DocumentModel(await NSwagDocument.LoadAsync(filePath));
                Documents.Add(document);
                SelectedDocument = document;
            }
        });
    }

    private async Task CloseAllDocumentsAsync(ObservableCollection<DocumentModel> documents)
    {
        foreach (var document in documents.ToList())
        {
            await CloseDocumentAsync(document);
        }
    }

    public async Task<bool> CloseDocumentAsync(DocumentModel document)
    {
        if (document.Document.IsDirty)
        {
            var result = await MessageBoxHelper.ShowYesNoCancel(
                "Do you want to save the file " + document.Document.Name + " ?",
                "Save file");

            if (result == ButtonResult.Yes)
            {
                var success = await SaveDocumentAsync(document);
                if (!success)
                    return false;
            }
            else if (result == ButtonResult.Cancel)
                return false;
        }

        Documents.Remove(document);
        return true;
    }

    private async Task<bool> SaveDocumentAsync(DocumentModel document)
    {
        return await RunTaskAsync(async () =>
        {
            if (File.Exists(document.Document.Path))
            {
                await document.Document.SaveAsync();
                await MessageBoxHelper.ShowInfo($"The file {document.Document.Name} has been saved.", "File saved");
                return true;
            }
            else
            {
                if (await SaveAsDocumentAsync(document))
                    return true;
            }

            return false;
        });
    }

    private async Task<bool> SaveAllDocumentAsync(ObservableCollection<DocumentModel> documents)
    {
        int changeCount = 0;
        foreach (var document in documents)
        {
            if (document.Document.IsDirty)
            {
                changeCount++;
                if (File.Exists(document.Document.Path))
                {
                    await document.Document.SaveAsync();
                }
                else
                {
                    if (!await SaveAsDocumentAsync(document))
                        return false;
                }
            }
        }

        if (changeCount > 0)
        {
            await MessageBoxHelper.ShowInfo($"{changeCount} changed file(s) saved.", "Files saved");
        }

        return true;
    }

    private async Task<bool> SaveAsDocumentAsync(DocumentModel document)
    {
        var window = DialogService.MainWindow;
        if (window == null)
            return false;

        var result = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save NSwag settings file",
            SuggestedFileName = document.Document.Name,
            FileTypeChoices = new[]
            {
                new FilePickerFileType("NSwag file") { Patterns = new[] { "*.nswag", "*nswag.json" } }
            }
        });

        if (result != null)
        {
            document.Document.Path = result.Path.LocalPath;
            await document.Document.SaveAsync();
            return true;
        }
        return false;
    }

    public void SaveWindowDocuments()
    {
        var paths = Documents
            .Where(d => File.Exists(d.Document.Path))
            .Select(d => d.Document.Path)
            .ToArray();
        ApplicationSettings.SetSetting("NSwagSettings", JsonConvert.SerializeObject(paths, Formatting.Indented));
    }
}
