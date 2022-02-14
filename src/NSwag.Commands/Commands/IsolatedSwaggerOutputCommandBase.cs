//-----------------------------------------------------------------------
// <copyright file="AssemblyOutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
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
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NJsonSchema.Yaml;
using NSwag.AssemblyLoader.Utilities;

namespace NSwag.Commands
{
    /// <summary>A command which is run in isolation.</summary>
    public abstract class IsolatedSwaggerOutputCommandBase<T> : IsolatedCommandBase<string>, IOutputCommand
        where T : JsonSchemaGeneratorSettings
    {
        [JsonIgnore]
        protected abstract T Settings { get; }

        [Argument(Name = "Output", IsRequired = false, Description = "The output file path (optional).")]
        [JsonProperty("output", NullValueHandling = NullValueHandling.Include)]
        public string OutputFilePath { get; set; }

        [Argument(Name = "OutputType", IsRequired = false, Description = "Specifies the output schema type, ignored when UseDocumentProvider is enabled (Swagger2|OpenApi3, default: Swagger2).")]
        public SchemaType OutputType
        {
            get { return Settings.SchemaType; }
            set { Settings.SchemaType = value; }
        }

        [Argument(Name = "NewLineBehavior", IsRequired = false, Description = "The new line behavior (Auto (OS default), CRLF, LF).")]
        [JsonProperty("newLineBehavior", NullValueHandling = NullValueHandling.Include)]
        public NewLineBehavior NewLineBehavior { get; set; } = NewLineBehavior.Auto;

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            JsonReferenceResolver ReferenceResolverFactory(OpenApiDocument d) =>
                new JsonAndYamlReferenceResolver(new JsonSchemaResolver(d, Settings));

            var documentJson = await RunIsolatedAsync((string)null);
            var document = await OpenApiDocument.FromJsonAsync(documentJson, null, OutputType, ReferenceResolverFactory).ConfigureAwait(false);
            await this.TryWriteDocumentOutputAsync(host, NewLineBehavior, () => document).ConfigureAwait(false);
            return document;
        }

        protected Assembly[] LoadAssemblies(IEnumerable<string> assemblyPaths, AssemblyLoader.AssemblyLoader assemblyLoader)
        {
#if NETFRAMEWORK
            var assemblies = PathUtilities.ExpandFileWildcards(assemblyPaths)
                .Select(path => Assembly.LoadFrom(path)).ToArray();
#else
            var currentDirectory = DynamicApis.DirectoryGetCurrentDirectory();
            var assemblies = PathUtilities.ExpandFileWildcards(assemblyPaths)
                .Select(path => assemblyLoader.Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(path, currentDirectory)))
                .ToArray();
#endif
            return assemblies;
        }
    }
}