//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MyToolkit.Command;
using MyToolkit.Storage;
using MyToolkit.Utilities;
using Newtonsoft.Json;

namespace NSwagStudio.ViewModels
{
    /// <summary>The view model for the MainWindow.</summary>
    public class MainWindowModel : ViewModelBase
    {
        private NSwagDocument _selectedDocument;

        /// <summary>Initializes a new instance of the <see cref="MainWindowModel"/> class.</summary>
        public MainWindowModel()
        {
            CreateDocumentCommand = new RelayCommand(CreateDocument);
            OpenDocumentCommand = new RelayCommand(OpenDocument);

            CloseDocumentCommand = new RelayCommand<NSwagDocument>(document => CloseDocument(document), document => document != null);
            SaveDocumentCommand = new RelayCommand<NSwagDocument>(document => SaveDocument(document), document => document != null);
            SaveAsDocumentCommand = new RelayCommand<NSwagDocument>(document => SaveAsDocument(document), document => document != null);

            Documents = new ObservableCollection<NSwagDocument>();
        }

        public ObservableCollection<NSwagDocument> Documents { get; private set; }

        /// <summary>Gets or sets the selected document. </summary>
        public NSwagDocument SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                if (Set(ref _selectedDocument, value))
                {
                    CloseDocumentCommand.RaiseCanExecuteChanged();
                    SaveDocumentCommand.RaiseCanExecuteChanged();
                    SaveAsDocumentCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand CreateDocumentCommand { get; private set; }

        public RelayCommand OpenDocumentCommand { get; private set; }

        public RelayCommand<NSwagDocument> CloseDocumentCommand { get; private set; }

        public RelayCommand<NSwagDocument> SaveDocumentCommand { get; private set; }

        public RelayCommand<NSwagDocument> SaveAsDocumentCommand { get; private set; }

        /// <summary>Gets the application version with build time. </summary>
        public string ApplicationVersion
        {
            get { return GetType().Assembly.GetVersionWithBuildTime(); }
        }

        protected override async void OnLoaded()
        {
            await Task.Delay(500);
            LoadApplicationSettings();
        }

        private void LoadApplicationSettings()
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
                            OpenDocument(path);

                        SelectedDocument = Documents.Last();
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
            var document = NSwagDocument.CreateDocument();
            Documents.Add(document);
            SelectedDocument = document;
        }

        private void OpenDocument()
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Open NSwag settings file";
            dlg.Filter = "NSwag settings (*.nswag)|*.nswag";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
                OpenDocument(dlg.FileName);
        }

        public void OpenDocument(string filePath)
        {
            try
            {
                var currentDocument = Documents.SingleOrDefault(d => d.Path == filePath);
                if (currentDocument != null)
                    SelectedDocument = currentDocument;
                else
                {
                    var document = NSwagDocument.LoadDocument(filePath);
                    Documents.Add(document);
                    SelectedDocument = document;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("File open failed: \n" + exception.Message, "Could not load the settings");
            }
        }

        public bool CloseDocument(NSwagDocument document)
        {
            if (document.IsDirty)
            {
                var result = MessageBox.Show("Do you want to save the file " + document.Name + " ?",
                    "Save file", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var success = SaveDocument(document);
                    if (!success)
                        return false;
                }
                else if (result == DialogResult.Cancel)
                    return false;
            }

            Documents.Remove(document);
            return true;
        }

        private bool SaveDocument(NSwagDocument document)
        {
            try
            {
                if (File.Exists(document.Path))
                {
                    document.Save();
                    MessageBox.Show("The file has been saved.", "File saved");
                    return true;
                }
                else
                {
                    if (SaveAsDocument(document))
                        return true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("File save failed: \n" + exception.Message, "Could not save the settings");
            }

            return false;
        }

        private bool SaveAsDocument(NSwagDocument document)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "NSwag settings (*.nswag)|*.nswag";
            dlg.RestoreDirectory = true;
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                document.Path = dlg.FileName;
                document.Save();
                return true;
            }
            return false;
        }
    }
}
