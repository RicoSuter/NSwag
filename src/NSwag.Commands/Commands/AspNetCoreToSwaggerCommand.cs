//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.Commands.Base;
using NSwag.Commands.SwaggerGeneration.AspNetCore;
using NSwag.SwaggerGeneration.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NSwag.Commands
{
    /// <summary>The generator.</summary>
    [Command(Name = "aspnetcore2swagger", Description = "Generates a Swagger specification ASP.NET Core Mvc application using ApiExplorer.")]
    public class AspNetCoreToSwaggerCommand : IConsoleCommand
    {
        [JsonIgnore]
        public AspNetCoreToSwaggerGeneratorCommandSettings Settings { get; } = new AspNetCoreToSwaggerGeneratorCommandSettings();

        [JsonIgnore]
        [Argument(Name = "Project", IsRequired = false, Description = "The project to use.")]
        public string Project { get; set; }

        [Argument(Name = "Controllers", IsRequired = false, Description = "The MSBuild project extensions path. Defaults to \"obj\".")]
        public string MSBuildProjectExtensionsPath { get; set; }

        [Argument(Name = "Configuration", IsRequired = false, Description = "The configuration to use.")]
        public string Configuration { get; set; }

        [Argument(Name = "Runtime", IsRequired = false, Description = "The runtime to use.")]
        public string Runtime { get; set; }

        [Argument(Name = "TargetFramework", IsRequired = false, Description = "The target framework to use.")]
        public string TargetFramework { get; set; }

        [Argument(Name = "DefaultPropertyNameHandling", IsRequired = false, Description = "The default property name handling ('Default' or 'CamelCase').")]
        public PropertyNameHandling DefaultPropertyNameHandling
        {
            get => Settings.DefaultPropertyNameHandling;
            set => Settings.DefaultPropertyNameHandling = value;
        }

        [Argument(Name = "DefaultReferenceTypeNullHandling", IsRequired = false, Description = "The default null handling (if NotNullAttribute and CanBeNullAttribute are missing, default: Null, Null or NotNull).")]
        public ReferenceTypeNullHandling DefaultReferenceTypeNullHandling
        {
            get => Settings.DefaultReferenceTypeNullHandling;
            set => Settings.DefaultReferenceTypeNullHandling = value;
        }

        [Argument(Name = "DefaultEnumHandling", IsRequired = false, Description = "The default enum handling ('String' or 'Integer'), default: Integer.")]
        public EnumHandling DefaultEnumHandling
        {
            get => Settings.DefaultEnumHandling;
            set => Settings.DefaultEnumHandling = value;
        }

        [Argument(Name = "FlattenInheritanceHierarchy", IsRequired = false, Description = "Flatten the inheritance hierarchy instead of using allOf to describe inheritance (default: false).")]
        public bool FlattenInheritanceHierarchy
        {
            get => Settings.FlattenInheritanceHierarchy;
            set => Settings.FlattenInheritanceHierarchy = value;
        }

        [Argument(Name = "GenerateKnownTypes", IsRequired = false, Description = "Generate schemas for types in KnownTypeAttribute attributes (default: true).")]
        public bool GenerateKnownTypes
        {
            get => Settings.GenerateKnownTypes;
            set => Settings.GenerateKnownTypes = value;
        }

        [Argument(Name = "GenerateXmlObjects", IsRequired = false, Description = "Generate xmlObject representation for definitions (default: false).")]
        public bool GenerateXmlObjects
        {
            get => Settings.GenerateXmlObjects;
            set => Settings.GenerateXmlObjects = value;
        }

        [Argument(Name = "GenerateAbstractProperties", IsRequired = false, Description = "Generate abstract properties (i.e. interface and abstract properties. Properties may defined multiple times in a inheritance hierarchy, default: false).")]
        public bool GenerateAbstractProperties
        {
            get => Settings.GenerateAbstractProperties;
            set => Settings.GenerateAbstractProperties = value;
        }

        [Argument(Name = "ServiceHost", IsRequired = false, Description = "Overrides the service host of the web service (optional, use '.' to remove the hostname).")]
        public string ServiceHost { get; set; }

        [Argument(Name = "ServiceBasePath", IsRequired = false, Description = "The basePath of the Swagger specification (optional).")]
        public string ServiceBasePath { get; set; }

        [Argument(Name = "ServiceSchemes", IsRequired = false, Description = "Overrides the allowed schemes of the web service (optional, comma separated, 'http', 'https', 'ws', 'wss').")]
        public string[] ServiceSchemes { get; set; }

        [Argument(Name = "InfoTitle", IsRequired = false, Description = "Specify the title of the Swagger specification.")]
        public string InfoTitle
        {
            get => Settings.Title;
            set => Settings.Title = value;
        }

        [Argument(Name = "InfoDescription", IsRequired = false, Description = "Specify the description of the Swagger specification.")]
        public string InfoDescription
        {
            get => Settings.Description;
            set => Settings.Description = value;
        }

        [Argument(Name = "InfoVersion", IsRequired = false, Description = "Specify the version of the Swagger specification (default: 1.0.0).")]
        public string InfoVersion
        {
            get => Settings.Version;
            set => Settings.Version = value;
        }

        [Argument(Name = "DocumentTemplate", IsRequired = false, Description = "Specifies the Swagger document template (may be a path or JSON, default: none).")]
        public string DocumentTemplate { get; set; }

        [Argument(Name = "DocumentProcessors", IsRequired = false, Description = "Gets the document processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string[] DocumentProcessorTypes
        {
            get => Settings.DocumentProcessorTypes;
            set => Settings.DocumentProcessorTypes = value;
        }

        [Argument(Name = "OperationProcessors", IsRequired = false, Description = "Gets the operation processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string[] OperationProcessorTypes
        {
            get => Settings.OperationProcessorTypes;
            set => Settings.OperationProcessorTypes = value;
        }

        public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            return await RunAsync();
        }

        public async Task<bool> RunAsync()
        {
            if (!string.IsNullOrEmpty(DocumentTemplate))
            {
                if (await DynamicApis.FileExistsAsync(DocumentTemplate).ConfigureAwait(false))
                    Settings.DocumentTemplate = await DynamicApis.FileReadAllTextAsync(DocumentTemplate).ConfigureAwait(false);
                else
                    Settings.DocumentTemplate = DocumentTemplate;
            }
            else
                Settings.DocumentTemplate = null;

            var projectFile = ProjectMetadata.FindProject(Project);
            var projectMetadata = ProjectMetadata.GetProjectMetadata(projectFile, MSBuildProjectExtensionsPath, TargetFramework, Runtime);
            if (string.IsNullOrEmpty(projectMetadata.NSwagExecutorDirectory))
            {
                throw new InvalidDataException($"Project '{projectFile}' does not reference to NSwag.Commands.AspNetCore package. A project must reference this package to execute the aspnetcore2swagger command.");
            }


            var args = new List<string>();


            var targetDir = Path.GetFullPath(Path.Combine(projectMetadata.ProjectDir, projectMetadata.OutputPath));
            var targetPath = Path.Combine(targetDir, projectMetadata.TargetFileName + ".json");

            string executable;
            if (projectMetadata.TargetFrameworkIdentifier == ".NETFramework")
            {
                throw new Exception("TBD");
            }
            else if (projectMetadata.TargetFrameworkIdentifier == ".NETCoreApp")
            {
                executable = "dotnet";
                args.Add("exec");
                args.Add("--depsfile");
                args.Add(projectMetadata.ProjectDepsFilePath);

                args.Add("--runtimeconfig");
                args.Add(projectMetadata.ProjectRuntimeConfigFilePath);

                var executorBinary = Path.Combine(projectMetadata.NSwagExecutorDirectory, "NSwag.Commands.AspNetCore.dll");
                args.Add(executorBinary);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported target framework '{projectMetadata.TargetFrameworkIdentifier}'.");
            }

            args.Add(projectMetadata.AssemblyName);
            args.Add(targetPath);

            var exitCode = Exe.Run(executable, args);
            return exitCode == 0;
        }
    }
}
