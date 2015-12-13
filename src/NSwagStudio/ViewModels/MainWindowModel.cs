//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Command;
using MyToolkit.Storage;
using MyToolkit.Utilities;
using Newtonsoft.Json;
using NSwagStudio.Views.ClientGenerators;
using NSwagStudio.Views.SwaggerGenerators;

namespace NSwagStudio.ViewModels
{
    /// <summary>The view model for the MainWindow.</summary>
    public class MainWindowModel : ViewModelBase
    {
        private ISwaggerGenerator _selectedSwaggerGenerator;
        private static NSwagSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="MainWindowModel"/> class.</summary>
        public MainWindowModel()
        {
            GenerateCommand = new AsyncRelayCommand(GenerateAsync);
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

        /// <summary>Gets or sets the settings. </summary>
        public static NSwagSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

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
            LoadSettings();

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

            SelectedSwaggerGenerator = SwaggerGenerators.First();
            RaiseAllPropertiesChanged();
        }

        protected override void OnUnloaded()
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            var settings = ApplicationSettings.GetSetting("NSwagSettings", string.Empty);
            if (!string.IsNullOrEmpty(settings))
            {
                try
                {
                    Settings = JsonConvert.DeserializeObject<NSwagSettings>(settings);
                }
                catch
                {
                    Settings = new NSwagSettings();
                }
            }
            else
                Settings = new NSwagSettings();
        }

        private void SaveSettings()
        {
            var settings = JsonConvert.SerializeObject(Settings);
            ApplicationSettings.SetSetting("NSwagSettings", settings);
        }
    }
}
