//-----------------------------------------------------------------------
// <copyright file="WebApiSwaggerGeneratorViewModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using MyToolkit.Command;
using MyToolkit.Storage;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwagStudio.ViewModels.SwaggerGenerators
{
    public class WebApiSwaggerGeneratorViewModel : ViewModelBase
    {
        private string _assemblyPath;
        private string _controllerName;
        private string[] _allControllerNames;
        private string _urlTemplate;

        /// <summary>Initializes a new instance of the <see cref="WebApiSwaggerGeneratorViewModel"/> class.</summary>
        public WebApiSwaggerGeneratorViewModel()
        {
            BrowseAssemblyCommand = new AsyncRelayCommand(BrowseAssembly);
            LoadAssemblyCommand = new AsyncRelayCommand(LoadAssembly, () => !string.IsNullOrEmpty(AssemblyPath));

            AssemblyPath = ApplicationSettings.GetSetting("AssemblyPath", string.Empty);
            UrlTemplate = ApplicationSettings.GetSetting("UrlTemplate", "api/{controller}/{action}/{id}");

            LoadAssemblyCommand.TryExecute();
        }

        /// <summary>Gets or sets the command to browse for an assembly.</summary>
        public AsyncRelayCommand BrowseAssemblyCommand { get; set; }

        /// <summary>Gets or sets the command to load the controller types from an assembly.</summary>
        public AsyncRelayCommand LoadAssemblyCommand { get; set; }

        /// <summary>Gets or sets the assembly path. </summary>
        public string AssemblyPath
        {
            get { return _assemblyPath; }
            set
            {
                if (Set(ref _assemblyPath, value))
                {
                    LoadAssemblyCommand.RaiseCanExecuteChanged();
                    ApplicationSettings.SetSetting("AssemblyPath", _assemblyPath);
                    RaisePropertyChanged(() => AssemblyName);
                }
            }
        }

        /// <summary>Gets the name of the selected assembly.</summary>
        public string AssemblyName
        {
            get { return Path.GetFileName(AssemblyPath); }
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

        /// <summary>Gets or sets the url template. </summary>
        public string UrlTemplate
        {
            get { return _urlTemplate; }
            set
            {
                if (Set(ref _urlTemplate, value))
                    ApplicationSettings.SetSetting("UrlTemplate", _urlTemplate);
            }
        }

        private bool GenerateFromWebApiCanExecute()
        {
            return !string.IsNullOrEmpty(AssemblyPath) &&
                !string.IsNullOrEmpty(ControllerName) &&
                !string.IsNullOrEmpty(UrlTemplate);
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
                    var generator = new WebApiAssemblyToSwaggerGenerator(AssemblyPath);
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
                    var generator = new WebApiAssemblyToSwaggerGenerator(AssemblyPath);
                    return generator.Generate(ControllerName, UrlTemplate).ToJson();
                });
            });
        }
    }
}
