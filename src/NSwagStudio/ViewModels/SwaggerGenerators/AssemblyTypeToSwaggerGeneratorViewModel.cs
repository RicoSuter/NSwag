//-----------------------------------------------------------------------
// <copyright file="AssemblyTypeToSwaggerGeneratorViewModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using MyToolkit.Command;
using NJsonSchema;
using NSwag;
using NSwag.Commands;
using NSwag.Commands.SwaggerGeneration;

namespace NSwagStudio.ViewModels.SwaggerGenerators
{
    public class AssemblyTypeToSwaggerGeneratorViewModel : ViewModelBase
    {
        private string[] _allClassNames;
        private TypesToSwaggerCommand _command = new TypesToSwaggerCommand();
        private NSwagDocument _document;

        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerGeneratorViewModel"/> class.</summary>
        public AssemblyTypeToSwaggerGeneratorViewModel()
        {
            BrowseAssemblyCommand = new AsyncRelayCommand(BrowseAssembly);

            LoadAssembliesCommand = new AsyncRelayCommand(async () => await LoadAssembliesAsync(), () => AssemblyPaths?.Length > 0);
            LoadAssembliesCommand.TryExecute();
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public TypesToSwaggerCommand Command
        {
            get { return _command; }
            set
            {
                if (Set(ref _command, value))
                    RaiseAllPropertiesChanged();
            }
        }

        /// <summary>Gets or sets the document.</summary>
        public NSwagDocument Document
        {
            get { return _document; }
            set
            {
                if (Set(ref _document, value))
                {
                    LoadAssembliesCommand.RaiseCanExecuteChanged();
                    LoadAssembliesAsync();
                }
            }
        }

        /// <summary>Gets or sets the reference path. </summary>
        public string ReferencePaths
        {
            get
            {
                return Command.ReferencePaths != null ? string.Join(",", Command.ReferencePaths) : "";
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Command.ReferencePaths = value.Split(',').Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n)).ToArray();
                else
                    Command.ReferencePaths = new string[] { };
                RaisePropertyChanged(() => ReferencePaths);
            }
        }

        /// <summary>Gets the default enum handlings. </summary>
        public EnumHandling[] EnumHandlings { get; } = Enum.GetNames(typeof(EnumHandling))
            .Select(t => (EnumHandling)Enum.Parse(typeof(EnumHandling), t))
            .ToArray();

        /// <summary>Gets the default property name handlings. </summary>
        public PropertyNameHandling[] PropertyNameHandlings { get; } = Enum.GetNames(typeof(PropertyNameHandling))
            .Select(t => (PropertyNameHandling)Enum.Parse(typeof(PropertyNameHandling), t))
            .ToArray();

        /// <summary>Gets the reference type null handlings. </summary>
        public ReferenceTypeNullHandling[] ReferenceTypeNullHandlings { get; } = Enum.GetNames(typeof(ReferenceTypeNullHandling))
            .Select(t => (ReferenceTypeNullHandling)Enum.Parse(typeof(ReferenceTypeNullHandling), t))
            .ToArray();

        /// <summary>Gets the output types. </summary>
        public SchemaType[] OutputTypes { get; } = { SchemaType.Swagger2, SchemaType.OpenApi3 };

        /// <summary>Gets or sets the command to browse for an assembly.</summary>
        public AsyncRelayCommand BrowseAssemblyCommand { get; set; }

        /// <summary>Gets or sets the command to load the types from an assembly.</summary>
        public AsyncRelayCommand LoadAssembliesCommand { get; set; }

        /// <summary>Gets or sets the assembly path. </summary>
        public string[] AssemblyPaths
        {
            get { return Command.AssemblyPaths; }
            set
            {
                Command.AssemblyPaths = value;
                LoadAssembliesCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => AssemblyPaths);
                RaisePropertyChanged(() => AssemblyName);
            }
        }

        /// <summary>Gets the name of the selected assembly.</summary>
        public string AssemblyName => Path.GetFileName(AssemblyPaths.FirstOrDefault());

        /// <summary>Gets or sets the class names. </summary>
        public IEnumerable<string> ClassNames
        {
            get { return Command.ClassNames; }
            set
            {
                Command.ClassNames = value.ToArray();
                RaisePropertyChanged(() => ClassNames);
            }
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
            dlg.Filter = ".NET Assemblies (*.dll;*.exe)|*.dll;*.exe";
            if (dlg.ShowDialog() == true)
            {
                AssemblyPaths = new[] { dlg.FileName };
                await LoadAssembliesAsync();
            }
        }

        private Task LoadAssembliesAsync()
        {
            return RunTaskAsync(async () =>
            {
                AllClassNames = await Task.Run(async () => await Document.GetTypesFromCommandLineAsync());
            });
        }

        public async Task<string> GenerateSwaggerAsync()
        {
            return await RunTaskAsync(async () =>
            {
                return await Task.Run(async () =>
                {
                    var document = (SwaggerDocument)await Command.RunAsync(null, null).ConfigureAwait(false);
                    return document?.ToJson();
                });
            });
        }
    }
}