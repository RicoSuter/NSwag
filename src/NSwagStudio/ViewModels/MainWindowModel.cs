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
            OpenDocumentCommand = new RelayCommand(OpenDocument);
            CloseDocumentCommand = new RelayCommand<NSwagDocument>(CloseDocument);

            Documents = new ObservableCollection<NSwagDocument>();
        }

        public ObservableCollection<NSwagDocument> Documents { get; private set; }

        /// <summary>Gets or sets the selected document. </summary>
        public NSwagDocument SelectedDocument
        {
            get { return _selectedDocument; }
            set { Set(ref _selectedDocument, value); }
        }

        public ICommand OpenDocumentCommand { get; private set; }

        public ICommand CloseDocumentCommand { get; private set; }

        /// <summary>Gets the application version with build time. </summary>
        public string ApplicationVersion
        {
            get { return GetType().Assembly.GetVersionWithBuildTime(); }
        }

        protected override void OnLoaded()
        {
            LoadApplicationSettings();
        }

        protected override void OnUnloaded()
        {
            SaveApplicationSettings();
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
                            LoadDocument(path);

                        SelectedDocument = Documents.Last();
                    }
                    else
                        CreateNewDocument();
                }
                else
                    CreateNewDocument();
            }
            catch
            {
                CreateNewDocument();
            }

            SelectedDocument = Documents.First();
        }

        private void CreateNewDocument()
        {
            var document = new NSwagDocument {Path = "Untitled"}; 
            Documents.Add(document);
            SelectedDocument = document; 
        }

        private void SaveApplicationSettings()
        {
            var paths = Documents
                .Where(d => File.Exists(d.Path))
                .Select(d => d.Path)
                .ToArray();

            ApplicationSettings.SetSetting("NSwagSettings", JsonConvert.SerializeObject(paths, Formatting.Indented));
        }

        private void OpenDocument()
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Open NSwag settings file";
            dlg.Filter = "NSwag settings (*.nswag)|*.nswag";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var document = LoadDocument(dlg.FileName);
                    SelectedDocument = document;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("File open failed: \n" + exception.Message, "Could not load the settings");
                }
            }
        }

        private NSwagDocument LoadDocument(string filePath)
        {
            var document = JsonConvert.DeserializeObject<NSwagDocument>(File.ReadAllText(filePath));
            document.Path = filePath;
            Documents.Add(document);
            return document;
        }

        private void CloseDocument(NSwagDocument document)
        {
            Documents.Remove(document);

            if (Documents.Count == 0)
                CreateNewDocument();
        }

    }
}
