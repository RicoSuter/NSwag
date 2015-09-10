//-----------------------------------------------------------------------
// <copyright file="AssemblySwaggerGeneratorViewModel.cs" company="NSwag">
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
using MyToolkit.Mvvm;
using MyToolkit.Storage;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwagStudio.ViewModels.SwaggerGenerators
{
    public class AssemblySwaggerGeneratorViewModel : ViewModelBase
    {
        private string _assemblyPath;
        private string _className;
        private string[] _allClassNames;

        public AssemblySwaggerGeneratorViewModel()
        {
            BrowseAssemblyCommand = new AsyncRelayCommand(BrowseAssembly);
            LoadAssemblyCommand = new AsyncRelayCommand(LoadAssembly, () => !string.IsNullOrEmpty(AssemblyPath));

            AssemblyPath = ApplicationSettings.GetSetting("AssemblyPath", string.Empty);
            LoadAssemblyCommand.TryExecute();
        }

        public AsyncRelayCommand BrowseAssemblyCommand { get; set; }

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

        public string AssemblyName
        {
            get { return Path.GetFileName(AssemblyPath); }
        }

        /// <summary>Gets or sets the class name. </summary>
        public string ClassName
        {
            get { return _className; }
            set { Set(ref _className, value); }
        }

        /// <summary>Gets or sets the all class names. </summary>
        public string[] AllClassNames
        {
            get { return _allClassNames; }
            set { Set(ref _allClassNames, value); }
        }

        private async Task BrowseAssembly()
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".dll"; 
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
                AllClassNames = await Task.Run(() =>
                {
                    var generator = new AssemblyTypeToSwaggerGenerator(AssemblyPath);
                    return generator.GetClasses();
                });
                ClassName = AllClassNames.FirstOrDefault();
            });
        }

        public async Task<string> GenerateSwaggerAsync()
        {
            return await RunTaskAsync(async () =>
            {
                return await Task.Run(() =>
                {
                    var generator = new AssemblyTypeToSwaggerGenerator(AssemblyPath);
                    return generator.FromAssemblyType(ClassName).ToJson();
                });
            });
        }
    }
}