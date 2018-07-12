//-----------------------------------------------------------------------
// <copyright file="AssemblyOutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.AssemblyLoader.Utilities;

namespace NSwag.Commands
{
    /// <summary>A command which is run in isolation.</summary>
    public abstract class IsolatedSwaggerOutputCommandBase : IsolatedCommandBase<string>, IOutputCommand
    {
        /// <summary>Initializes a new instance of the <see cref="IsolatedSwaggerOutputCommandBase"/> class.</summary>
        protected IsolatedSwaggerOutputCommandBase()
        {
            OutputType = SchemaType.Swagger2;
        }

        [Argument(Name = "Output", IsRequired = false, Description = "The output file path (optional).")]
        [JsonProperty("output", NullValueHandling = NullValueHandling.Include)]
        public string OutputFilePath { get; set; }

        [Argument(Name = "OutputType", IsRequired = false, Description = "Specifies the output schema type (Swagger2|OpenApi3, default: Swagger2).")]
        public SchemaType OutputType { get; set; }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var documentJson = await RunIsolatedAsync((string)null);
            var document = await SwaggerDocument.FromJsonAsync(documentJson, expectedSchemaType: OutputType).ConfigureAwait(false);
            await this.TryWriteDocumentOutputAsync(host, () => document).ConfigureAwait(false);
            return document;
        }

        protected async Task<Assembly[]> LoadAssembliesAsync(IEnumerable<string> assemblyPaths, AssemblyLoader.AssemblyLoader assemblyLoader)
        {
#if FullNet
            var assemblies = PathUtilities.ExpandFileWildcards(assemblyPaths)
                .Select(path => Assembly.LoadFrom(path)).ToArray();
#else
            var currentDirectory = await DynamicApis.DirectoryGetCurrentDirectoryAsync().ConfigureAwait(false);
            var assemblies = PathUtilities.ExpandFileWildcards(assemblyPaths)
                .Select(path => assemblyLoader.Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(path, currentDirectory)))
                .ToArray();
#endif
            return assemblies;
        }
    }
}