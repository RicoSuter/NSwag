//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyToolkit.Command;
using MyToolkit.Storage;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using NSwag.Commands;

namespace NSwagStudio.ViewModels
{
    using System.Windows;
    using System.Windows.Input;

    using MessageBox = System.Windows.Forms.MessageBox;

    /// <summary>The view model for the MainWindow.</summary>
    public class MainWindowModel : ViewModelBase
    {
        private DocumentModel _selectedDocument;

        /// <summary>Initializes a new instance of the <see cref="MainWindowModel"/> class.</summary>
        public MainWindowModel()
        {
            CreateDocumentCommand = new RelayCommand(CreateDocument);
            OpenDocumentCommand = new AsyncRelayCommand(OpenDocumentAsync);

            CloseDocumentCommand = new AsyncRelayCommand<DocumentModel>(async document => await CloseDocumentAsync(document), document => document != null);
            CloseAllDocumentsCommand = new AsyncRelayCommand<ObservableCollection<DocumentModel>>(async documents => await CloseAllDocumentsAsync(documents), documents => documents.Any());
            SaveDocumentCommand = new AsyncRelayCommand<DocumentModel>(async document => await SaveDocumentAsync(document), document => document != null);
            SaveAsDocumentCommand = new AsyncRelayCommand<DocumentModel>(async document => await SaveAsDocumentAsync(document), document => document != null);
            SaveAllDocumentsCommand = new AsyncRelayCommand<ObservableCollection<DocumentModel>>(async documents => await SaveAllDocumentAsync(documents), documents => documents.Any());

            Documents = new ObservableCollection<DocumentModel>();
        }

        public ObservableCollection<DocumentModel> Documents { get; private set; }

        /// <summary>Gets or sets the selected document. </summary>
        public DocumentModel SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                if (Set(ref _selectedDocument, value))
                {
                    CloseDocumentCommand.RaiseCanExecuteChanged();
                    CloseAllDocumentsCommand.RaiseCanExecuteChanged();
                    SaveDocumentCommand.RaiseCanExecuteChanged();
                    SaveAsDocumentCommand.RaiseCanExecuteChanged();
                    SaveAllDocumentsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand CreateDocumentCommand { get; }

        public AsyncRelayCommand OpenDocumentCommand { get; }

        public AsyncRelayCommand<DocumentModel> CloseDocumentCommand { get; }
        
        public AsyncRelayCommand<ObservableCollection<DocumentModel>> CloseAllDocumentsCommand { get; }

        public AsyncRelayCommand<DocumentModel> SaveDocumentCommand { get; }

        public AsyncRelayCommand<DocumentModel> SaveAsDocumentCommand { get; }

        public AsyncRelayCommand<ObservableCollection<DocumentModel>> SaveAllDocumentsCommand { get; }

        public string NSwagVersion => OpenApiDocument.ToolchainVersion;

        public string NJsonSchemaVersion => JsonSchema.ToolchainVersion;

        protected override async void OnLoaded()
        {
            await Task.Delay(500);
            await LoadApplicationSettingsAsync();
        }

        private async Task LoadApplicationSettingsAsync()
        {
            try
            {
                var settings = ApplicationSettings.GetSetting("NSwagSettings", string.Empty);
                if (settings != string.Empty)
                {
                    var paths = JsonConvert.DeserializeObject<string[]>(settings)
                        .Where(File.Exists)
                        .ToArray();

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

            SelectedDocument = Documents.First();
        }

        private void CreateDocument()
        {
            var document = new DocumentModel(NSwagDocument.Create());
            Documents.Add(document);
            SelectedDocument = document;
        }

        private async Task OpenDocumentAsync()
        {
            var dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Title = "Open NSwag settings file";
            dlg.Filter = "NSwag file (*.nswag;*nswag.json)|*.nswag;*nswag.json";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileName in dlg.FileNames)
                {
                    await OpenDocumentAsync(fileName);
                }
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
                var result = MessageBox.Show("Do you want to save the file " + document.Document.Name + " ?",
                    "Save file", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = await SaveDocumentAsync(document);
                    if (!success)
                        return false;
                }
                else if (result == DialogResult.Cancel)
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
                    FocusManager.SetFocusedElement(Application.Current.MainWindow, null);
                    await document.Document.SaveAsync();
                    MessageBox.Show($"The file {document.Document.Name} has been saved.", "File saved");
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
            FocusManager.SetFocusedElement(Application.Current.MainWindow, null);
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
                MessageBox.Show($"{changeCount} changed file(s) saved.", "Files saved");
            }

            return true;
        }

        private async Task<bool> SaveAsDocumentAsync(DocumentModel document)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "NSwag file (*.nswag;nswag.json)|*.nswag;nswag.json";
            dlg.RestoreDirectory = true;
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                document.Document.Path = dlg.FileName;
                FocusManager.SetFocusedElement(Application.Current.MainWindow, null);
                await document.Document.SaveAsync();
                return true;
            }
            return false;
        }
    }
}
