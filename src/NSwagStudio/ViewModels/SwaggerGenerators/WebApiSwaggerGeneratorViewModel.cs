//-----------------------------------------------------------------------
// <copyright file="WebApiSwaggerGeneratorViewModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using MyToolkit.Command;
using MyToolkit.Storage;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwagStudio.ViewModels.SwaggerGenerators
{
    public class WebApiSwaggerGeneratorViewModel : ViewModelBase
    {
        private string _controllerName;
        private string[] _allControllerNames;
        private bool _specifyControllerName;

        /// <summary>Initializes a new instance of the <see cref="WebApiSwaggerGeneratorViewModel"/> class.</summary>
        public WebApiSwaggerGeneratorViewModel()
        {
            BrowseAssemblyCommand = new AsyncRelayCommand(BrowseAssembly);
            SpecifyControllerName = true;

            Settings = JsonConvert.DeserializeObject<WebApiAssemblyToSwaggerGeneratorSettings>(
                ApplicationSettings.GetSetting("WebApiAssemblyToSwaggerGeneratorSettings",
                JsonConvert.SerializeObject(new WebApiAssemblyToSwaggerGeneratorSettings())));

            LoadAssemblyCommand = new AsyncRelayCommand(LoadAssembly, () => !string.IsNullOrEmpty(AssemblyPath));
            LoadAssemblyCommand.TryExecute();
        }

        protected override void OnUnloaded()
        {
            ApplicationSettings.SetSetting("WebApiAssemblyToSwaggerGeneratorSettings", JsonConvert.SerializeObject(Settings));
        }
        
        /// <summary>Gets the default enum handling. </summary>
        public EnumHandling[] EnumHandlings
        {
            get { return Enum.GetNames(typeof(EnumHandling)).Select(t => (EnumHandling)Enum.Parse(typeof(EnumHandling), t)).ToArray(); }
        }

        /// <summary>Gets or sets the command to browse for an assembly.</summary>
        public AsyncRelayCommand BrowseAssemblyCommand { get; set; }

        /// <summary>Gets or sets the command to load the controller types from an assembly.</summary>
        public AsyncRelayCommand LoadAssemblyCommand { get; set; }

        /// <summary>Gets or sets the assembly path. </summary>
        public string AssemblyPath
        {
            get { return Settings.AssemblyPath; }
            set
            {
                Settings.AssemblyPath = value;
                LoadAssemblyCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => AssemblyPath);
                RaisePropertyChanged(() => AssemblyName);
            }
        }

        /// <summary>Gets the generator settings.</summary>
        public WebApiAssemblyToSwaggerGeneratorSettings Settings { get; private set; }

        /// <summary>Gets the name of the selected assembly.</summary>
        public string AssemblyName
        {
            get { return Path.GetFileName(AssemblyPath); }
        }

        /// <summary>Gets or sets a value indicating whether to specify a single controller name. </summary>
        public bool SpecifyControllerName
        {
            get { return _specifyControllerName; }
            set { Set(ref _specifyControllerName, value); }
        }

        /// <summary>Gets or sets the class name. </summary>
        public string ControllerName
        {
            get { return _controllerName; }
            set { Set(ref _controllerName, value); }
        }

        /// <summary>Gets or sets the all class names. </summary>
        public string[] AllControllerNames
        {
            get { return _allControllerNames; }
            set { Set(ref _allControllerNames, value); }
        }       

        private async Task BrowseAssembly()
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".dll"; // 
            dlg.Filter = ".NET Assemblies (.dll)|*.dll"; 
            if (dlg.ShowDialog() == true)
            {
                AssemblyPath = dlg.FileName;
                await LoadAssembly();
            }
        }

        private Task LoadAssembly()
        {
            return RunTaskAsync(async () =>
            {
                AllControllerNames = await Task.Run(() =>
                {
                    var generator = new WebApiAssemblyToSwaggerGenerator(Settings);
                    return generator.GetControllerClasses();
                });

                ControllerName = AllControllerNames.FirstOrDefault();
            });
        }

        public async Task<string> GenerateSwaggerAsync()
        {
            return await RunTaskAsync(async () =>
            {
                return await Task.Run(() =>
                {
                    var generator = new WebApiAssemblyToSwaggerGenerator(Settings);
                    if (SpecifyControllerName)
                        return generator.GenerateForSingleController(ControllerName).ToJson();
                    else
                        return generator.GenerateForAssemblyControllers().ToJson();
                });
            });
        }
    }
}
