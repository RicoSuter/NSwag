//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MyToolkit.Command;
using MyToolkit.Resources;
using MyToolkit.Storage;
using MyToolkit.Utilities;
using Newtonsoft.Json;
using NSwag.CodeGeneration.ClientGenerators.CSharp;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwagStudio.Views.ClientGenerators;
using NSwagStudio.Views.SwaggerGenerators;

namespace NSwagStudio.ViewModels
{
    /// <summary>The view model for the MainWindow.</summary>
    public class MainWindowModel : ViewModelBase
    {
        private static NSwagSettings _settings;

        private ISwaggerGenerator _selectedSwaggerGenerator;
        private IClientGenerator _selectedClientGenerator;

        /// <summary>Initializes a new instance of the <see cref="MainWindowModel"/> class.</summary>
        public MainWindowModel()
        {
            GenerateCommand = new AsyncRelayCommand(GenerateAsync);
            LoadSettingsCommand = new RelayCommand(LoadSettings);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        /// <summary>Gets or sets the command to generate code from the selected Swagger generator.</summary>
        public AsyncRelayCommand GenerateCommand { get; set; }

        /// <summary>Gets the swagger generators.</summary>
        public ISwaggerGenerator[] SwaggerGenerators { get; private set; }

        /// <summary>Gets the client generators.</summary>
        public IClientGenerator[] ClientGenerators { get; private set; }

        /// <summary>Gets or sets the selected <see cref="ISwaggerGenerator"/>. </summary>
        public ISwaggerGenerator SelectedSwaggerGenerator
        {
            get { return _selectedSwaggerGenerator; }
            set { Set(ref _selectedSwaggerGenerator, value); }
        }

        /// <summary>Gets or sets the selected <see cref="IClientGenerator"/>. </summary>
        public IClientGenerator SelectedClientGenerator
        {
            get { return _selectedClientGenerator; }
            set { Set(ref _selectedClientGenerator, value); }
        }

        /// <summary>Gets or sets the settings. </summary>
        public static NSwagSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public ICommand LoadSettingsCommand { get; private set; }

        public ICommand SaveSettingsCommand { get; private set; }

        /// <summary>Gets the application version with build time. </summary>
        public string ApplicationVersion
        {
            get { return GetType().Assembly.GetVersionWithBuildTime(); }
        }

        private async Task GenerateAsync()
        {
            if (SelectedSwaggerGenerator != null)
            {
                var swaggerCode = await SelectedSwaggerGenerator.GenerateSwaggerAsync();
                foreach (var generator in ClientGenerators)
                    await generator.GenerateClientAsync(swaggerCode);
            }
        }

        protected override void OnLoaded()
        {
            LoadApplicationSettings();

            SwaggerGenerators = new ISwaggerGenerator[]
            {
                new SwaggerInputGeneratorView(),
                new WebApiSwaggerGeneratorView(),
                new JsonSchemaInputGeneratorView(),
                new AssemblySwaggerGeneratorView(),
            };

            ClientGenerators = new IClientGenerator[]
            {
                new SwaggerGeneratorView(),
                new TypeScriptCodeGeneratorView(),
                new CSharpClientGeneratorView()
            };

            RaisePropertyChanged(() => SwaggerGenerators);
            RaisePropertyChanged(() => ClientGenerators);

            SelectedSwaggerGenerator = SwaggerGenerators.First();
            SelectedClientGenerator = ClientGenerators.First();
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
                    Settings = JsonConvert.DeserializeObject<NSwagSettings>(settings);
                else
                    Settings = new NSwagSettings();
            }
            catch
            {
                Settings = new NSwagSettings();
            }
        }

        private void SaveApplicationSettings()
        {
            ApplicationSettings.SetSetting("NSwagSettings", JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }

        private void LoadSettings()
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Open NSwag settings file";
            dlg.Filter = "NSwag settings (*.nswag)|*.nswag";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Settings = JsonConvert.DeserializeObject<NSwagSettings>(File.ReadAllText(dlg.FileName));
                }
                catch (Exception exception)
                {
                    MessageBox.Show("File open failed: \n" + exception.Message, "Could not load the settings");
                }
            }
        }

        private void SaveSettings()
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "NSwag settings (*.nswag)|*.nswag";
            dlg.RestoreDirectory = true;
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(dlg.FileName, JsonConvert.SerializeObject(Settings));
                }
                catch (Exception exception)
                {
                    MessageBox.Show("File save failed: \n" + exception.Message, "Could not save the settings");
                }
            }
        }
    }
}
