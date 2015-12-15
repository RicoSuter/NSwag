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
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Commands;

namespace NSwagStudio.ViewModels.SwaggerGenerators
{
    public class WebApiSwaggerGeneratorViewModel : ViewModelBase
    {
        private bool _specifyControllerName;
        private string[] _allControllerNames;
        private WebApiToSwaggerCommand _command = MainWindowModel.Settings.WebApiToSwaggerCommand;

        /// <summary>Initializes a new instance of the <see cref="WebApiSwaggerGeneratorViewModel"/> class.</summary>
        public WebApiSwaggerGeneratorViewModel()
        {
            BrowseAssemblyCommand = new AsyncRelayCommand(BrowseAssembly);
            SpecifyControllerName = true; 

            LoadAssemblyCommand = new AsyncRelayCommand(LoadAssembly, () => !string.IsNullOrEmpty(AssemblyPath));
            LoadAssemblyCommand.TryExecute();
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

        /// <summary>Gets or sets the generator settings. </summary>
        public WebApiToSwaggerCommand Command
        {
            get { return _command; }
            set { Set(ref _command, value); }
        }

        /// <summary>Gets or sets the assembly path. </summary>
        public string AssemblyPath
        {
            get { return Command.AssemblyPath; }
            set
            {
                Command.AssemblyPath = value;
                LoadAssemblyCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => AssemblyPath);
                RaisePropertyChanged(() => AssemblyName);
            }
        }

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
            get { return Command.ControllerName; }
            set { Command.ControllerName = value; }
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

        public async Task<string> GenerateSwaggerAsync()
        {
            return await RunTaskAsync(async () => await Task.Run(async () =>
            {
                if (!SpecifyControllerName)
                    Command.ControllerName = null; 

                return await Command.RunAsync();
            }));
        }

        private Task LoadAssembly()
        {
            return RunTaskAsync(async () =>
            {
                AllControllerNames = await Task.Run(() =>
                {
                    var generator = new WebApiAssemblyToSwaggerGenerator(Command.Settings);
                    return generator.GetControllerClasses();
                });

                if (!AllControllerNames.Contains(ControllerName))
                    ControllerName = AllControllerNames.FirstOrDefault();
            });
        }
    }
}
