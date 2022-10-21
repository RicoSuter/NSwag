//-----------------------------------------------------------------------
// <copyright file="TypesToSwaggerCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.AssemblyLoader.Utilities;
using NSwag.Generation.AspNetCore;

namespace NSwag.Commands.Generation
{
    /// <summary></summary>
    [Command(Name = "types2openapi")]
    public class TypesToOpenApiCommand : TypesToSwaggerCommand
    {
    }

    /// <summary></summary>
    [Command(Name = "types2swagger")]
    public class TypesToSwaggerCommand : IsolatedSwaggerOutputCommandBase<AspNetCoreOpenApiDocumentGeneratorSettings>
    {
        /// <summary>Initializes a new instance of the <see cref="TypesToSwaggerCommand"/> class.</summary>
        public TypesToSwaggerCommand()
        {
            ClassNames = new string[] { };
        }

        [JsonIgnore]
        protected override AspNetCoreOpenApiDocumentGeneratorSettings Settings { get; }

        [Argument(Name = "ClassNames", Description = "The class names.")]
        public string[] ClassNames { get; set; }

        [Argument(Name = nameof(DefaultReferenceTypeNullHandling), IsRequired = false, Description = "The default reference type null handling (Null (default) or NotNull).")]
        public ReferenceTypeNullHandling DefaultReferenceTypeNullHandling
        {
            get => Settings.SchemaSettings.DefaultReferenceTypeNullHandling;
            set => Settings.SchemaSettings.DefaultReferenceTypeNullHandling = value;
        }

        [Argument(Name = nameof(DefaultDictionaryValueReferenceTypeNullHandling), IsRequired = false, Description = "The default reference type null handling of dictionary value types (NotNull (default) or Null).")]
        public ReferenceTypeNullHandling DefaultDictionaryValueReferenceTypeNullHandling
        {
            get => Settings.SchemaSettings.DefaultDictionaryValueReferenceTypeNullHandling;
            set => Settings.SchemaSettings.DefaultDictionaryValueReferenceTypeNullHandling = value;
        }

        [Argument(Name = "FlattenInheritanceHierarchy", IsRequired = false, Description = "Flatten the inheritance hierarchy instead of using allOf to describe inheritance (default: false).")]
        public bool FlattenInheritanceHierarchy
        {
            get { return Settings.SchemaSettings.FlattenInheritanceHierarchy; }
            set { Settings.SchemaSettings.FlattenInheritanceHierarchy = value; }
        }

        [Argument(Name = "IgnoreObsoleteProperties", IsRequired = false, Description = "Ignore properties with the ObsoleteAttribute (default: false).")]
        public bool IgnoreObsoleteProperties
        {
            get { return Settings.SchemaSettings.IgnoreObsoleteProperties; }
            set { Settings.SchemaSettings.IgnoreObsoleteProperties = value; }
        }

        [Argument(Name = "AllowReferencesWithProperties", IsRequired = false, Description = "Use $ref references even if additional properties are defined on " +
            "the object (otherwise allOf/oneOf with $ref is used, default: false).")]
        public bool AllowReferencesWithProperties
        {
            get { return Settings.SchemaSettings.AllowReferencesWithProperties; }
            set { Settings.SchemaSettings.AllowReferencesWithProperties = value; }
        }

        [Argument(Name = "GenerateKnownTypes", IsRequired = false, Description = "Generate schemas for types in KnownTypeAttribute attributes (default: true).")]
        public bool GenerateKnownTypes
        {
            get { return Settings.SchemaSettings.GenerateKnownTypes; }
            set { Settings.SchemaSettings.GenerateKnownTypes = value; }
        }

        [Argument(Name = "GenerateEnumMappingDescription", IsRequired = false,
            Description = "Generate a description with number to enum name mappings (for integer enums only, default: false).")]
        public bool GenerateEnumMappingDescription
        {
            get => Settings.SchemaSettings.GenerateEnumMappingDescription;
            set => Settings.SchemaSettings.GenerateEnumMappingDescription = value;
        }

        [Argument(Name = "GenerateXmlObjects", IsRequired = false, Description = "Generate xmlObject representation for definitions (default: false).")]
        public bool GenerateXmlObjects
        {
            get { return Settings.SchemaSettings.GenerateXmlObjects; }
            set { Settings.SchemaSettings.GenerateXmlObjects = value; }
        }

        [Argument(Name = "UseXmlDocumentation", IsRequired = false, Description = "Read XML Docs files (default: true).")]
        public bool UseXmlDocumentation
        {
            get => Settings.SchemaSettings.UseXmlDocumentation;
            set => Settings.SchemaSettings.UseXmlDocumentation = value;
        }

        [Argument(Name = "ResolveExternalXmlDocumentation", IsRequired = false, Description = "Resolve the XML Docs from the NuGet cache or .NET SDK directory (default: true).")]
        public bool ResolveExternalXmlDocumentation
        {
            get => Settings.SchemaSettings.ResolveExternalXmlDocumentation;
            set => Settings.SchemaSettings.ResolveExternalXmlDocumentation = value;
        }

        protected override Task<string> RunIsolatedAsync(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
            var document = new OpenApiDocument();
            var generator = new JsonSchemaGenerator(Settings.SchemaSettings);
            var schemaResolver = new OpenApiSchemaResolver(document, Settings.SchemaSettings);

#if NETFRAMEWORK
            var assemblies = PathUtilities.ExpandFileWildcards(AssemblyPaths)
                .Select(path => Assembly.LoadFrom(path)).ToArray();
#else
            var currentDirectory = DynamicApis.DirectoryGetCurrentDirectory();
            var assemblies = PathUtilities.ExpandFileWildcards(AssemblyPaths)
                .Select(path => assemblyLoader.Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(path, currentDirectory))).ToArray();
#endif

            var allExportedClassNames = assemblies.SelectMany(a => a.ExportedTypes).Select(t => t.FullName).ToList();
            var matchedClassNames = ClassNames
                .SelectMany(n => PathUtilities.FindWildcardMatches(n, allExportedClassNames, '.'))
                .Distinct();

            foreach (var className in matchedClassNames)
            {
                var type = assemblies.Select(a => a.GetType(className)).FirstOrDefault(t => t != null);
                generator.Generate(type, schemaResolver);
            }

            return Task.FromResult(document.ToJson(OutputType));
        }
    }
}