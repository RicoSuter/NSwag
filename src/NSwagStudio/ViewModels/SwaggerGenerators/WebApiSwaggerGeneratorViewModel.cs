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
        private string[] _allControllerNames = { };
        private WebApiToSwaggerCommand _command = new WebApiToSwaggerCommand();

        /// <summary>Initializes a new instance of the <see cref="WebApiSwaggerGeneratorViewModel"/> class.</summary>
        public WebApiSwaggerGeneratorViewModel()
        {
            BrowseAssemblyCommand = new AsyncRelayCommand(BrowseAssembly);

            LoadAssemblyCommand = new AsyncRelayCommand(async () => await LoadAssemblyAsync(true), () => !string.IsNullOrEmpty(AssemblyPath));
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
            set
            {
                if (Set(ref _command, value))
                {
                    RaiseAllPropertiesChanged();
                    LoadAssemblyCommand.RaiseCanExecuteChanged();
                    LoadAssemblyAsync(false);
                }
            }
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
            get { return Command.ControllerName != null; }
            set
            {
                if (value != SpecifyControllerName)
                    ControllerName = value ? AllControllerNames.FirstOrDefault() : null;

                RaisePropertyChanged(() => SpecifyControllerName);
            }
        }

        /// <summary>Gets or sets the class name. </summary>
        public string ControllerName
        {
            get { return Command.ControllerName; }
            set
            {
                Command.ControllerName = value;
                RaisePropertyChanged(() => ControllerName);
                RaisePropertyChanged(() => SpecifyControllerName);
            }
        }

        /// <summary>Gets or sets the all class names. </summary>
        public string[] AllControllerNames
        {
            get { return _allControllerNames; }
            set { Set(ref _allControllerNames, value); }
        }

        public async Task<string> GenerateSwaggerAsync()
        {
            return await RunTaskAsync(async () => await Task.Run(async () =>
            {
                return await Command.RunAsync();
            }));
        }

        private async Task BrowseAssembly()
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".dll"; // 
            dlg.Filter = ".NET Assemblies (.dll)|*.dll";
            if (dlg.ShowDialog() == true)
            {
                AssemblyPath = dlg.FileName;
                await LoadAssemblyAsync(true);
            }
        }

        private Task LoadAssemblyAsync(bool updateSelectedController)
        {
            return RunTaskAsync(async () =>
            {
                AllControllerNames = await Task.Run(() =>
                {
                    var generator = new WebApiAssemblyToSwaggerGenerator(Command.Settings);
                    return generator.GetControllerClasses();
                });

                if (updateSelectedController)
                {
                    if (ControllerName != null && !AllControllerNames.Contains(ControllerName))
                        ControllerName = AllControllerNames.FirstOrDefault();
                }
            });
        }
    }
}
