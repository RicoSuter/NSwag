//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.Processors;

namespace NSwag.Commands.SwaggerGeneration
{
    /// <inheritdoc />
    public abstract class SwaggerGeneratorCommandBase<T> : IsolatedSwaggerOutputCommandBase
        where T : SwaggerGeneratorSettings, new()
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerGeneratorCommandBase{T}"/> class.</summary>
        protected SwaggerGeneratorCommandBase()
        {
            Settings = new T();
        }

        [JsonIgnore]
        public T Settings { get; }

        [Argument(Name = nameof(DefaultPropertyNameHandling), IsRequired = false, Description = "The default property name handling ('Default' or 'CamelCase').")]
        public PropertyNameHandling DefaultPropertyNameHandling
        {
            get => Settings.DefaultPropertyNameHandling;
            set => Settings.DefaultPropertyNameHandling = value;
        }

        [Argument(Name = nameof(DefaultReferenceTypeNullHandling), IsRequired = false, Description = "The default null handling (if NotNullAttribute and CanBeNullAttribute are missing, default: Null, Null or NotNull).")]
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

        [Argument(Name = "GenerateAbstractProperties", IsRequired = false, Description = "Generate abstract properties (i.e. interface and abstract properties. " +
            "Properties may defined multiple times in a inheritance hierarchy, default: false).")]
        public bool GenerateAbstractProperties
        {
            get => Settings.GenerateAbstractProperties;
            set => Settings.GenerateAbstractProperties = value;
        }

        [Argument(Name = "IgnoreObsoleteProperties", IsRequired = false, Description = "Ignore properties with the ObsoleteAttribute (default: false).")]
        public bool IgnoreObsoleteProperties
        {
            get => Settings.IgnoreObsoleteProperties;
            set => Settings.IgnoreObsoleteProperties = value;
        }

        [Argument(Name = "AllowReferencesWithProperties", IsRequired = false, Description = "Use $ref references even if additional properties are defined on " +
            "the object (otherwise allOf/oneOf with $ref is used, default: false).")]
        public bool AllowReferencesWithProperties
        {
            get => Settings.AllowReferencesWithProperties;
            set => Settings.AllowReferencesWithProperties = value;
        }

        [Argument(Name = "ExcludedTypeNames", IsRequired = false, Description = "The excluded type names (same as JsonSchemaIgnoreAttribute).")]
        public string[] ExcludedTypeNames
        {
            get => Settings.ExcludedTypeNames;
            set => Settings.ExcludedTypeNames = value;
        }

        [Argument(Name = "ServiceHost", IsRequired = false, Description = "Overrides the service host of the web service (optional, use '.' to remove the hostname).")]
        public string ServiceHost { get; set; }

        [Argument(Name = "ServiceBasePath", IsRequired = false, Description = "The basePath of the Swagger specification (optional).")]
        public string ServiceBasePath { get; set; }

        [Argument(Name = "ServiceSchemes", IsRequired = false, Description = "Overrides the allowed schemes of the web service (optional, comma separated, 'http', 'https', 'ws', 'wss').")]
        public string[] ServiceSchemes { get; set; }

        [Argument(Name = "InfoTitle", IsRequired = false, Description = "Specify the title of the Swagger specification (ignored when DocumentTemplate is set).")]
        public string InfoTitle
        {
            get => Settings.Title;
            set => Settings.Title = value;
        }

        [Argument(Name = "InfoDescription", IsRequired = false, Description = "Specify the description of the Swagger specification (ignored when DocumentTemplate is set).")]
        public string InfoDescription
        {
            get => Settings.Description;
            set => Settings.Description = value;
        }

        [Argument(Name = "InfoVersion", IsRequired = false, Description = "Specify the version of the Swagger specification (default: 1.0.0, ignored when DocumentTemplate is set).")]
        public string InfoVersion
        {
            get => Settings.Version;
            set => Settings.Version = value;
        }

        [Argument(Name = "DocumentTemplate", IsRequired = false, Description = "Specifies the Swagger document template (may be a path or JSON, default: none).")]
        public string DocumentTemplate { get; set; }

        [Argument(Name = "DocumentProcessors", IsRequired = false, Description = "The document processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string[] DocumentProcessorTypes { get; set; }

        [Argument(Name = "OperationProcessors", IsRequired = false, Description = "The operation processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string[] OperationProcessorTypes { get; set; }

        [Argument(Name = "TypeNameGenerator", IsRequired = false, Description = "The custom ITypeNameGenerator implementation type in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string TypeNameGeneratorType { get; set; }

        [Argument(Name = "SchemaNameGenerator", IsRequired = false, Description = "The custom ISchemaNameGenerator implementation type in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string SchemaNameGeneratorType { get; set; }

        [Argument(Name = "ContractResolver", IsRequired = false, Description = "The custom IContractResolver implementation type in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string ContractResolverType { get; set; }

        public void InitializeCustomTypes(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
            if (DocumentProcessorTypes != null)
            {
                foreach (var p in DocumentProcessorTypes)
                {
                    var processor = (IDocumentProcessor)assemblyLoader.CreateInstance(p);
                    Settings.DocumentProcessors.Add(processor);
                }
            }

            if (OperationProcessorTypes != null)
            {
                foreach (var p in OperationProcessorTypes)
                {
                    var processor = (IOperationProcessor)assemblyLoader.CreateInstance(p);
                    Settings.OperationProcessors.Add(processor);
                }
            }

            if (!string.IsNullOrEmpty(TypeNameGeneratorType))
                Settings.TypeNameGenerator = (ITypeNameGenerator)assemblyLoader.CreateInstance(TypeNameGeneratorType);

            if (!string.IsNullOrEmpty(SchemaNameGeneratorType))
                Settings.SchemaNameGenerator = (ISchemaNameGenerator)assemblyLoader.CreateInstance(SchemaNameGeneratorType);

            if (!string.IsNullOrEmpty(ContractResolverType))
                Settings.ContractResolver = (IContractResolver)assemblyLoader.CreateInstance(ContractResolverType);
        }

        public string PostprocessDocument(SwaggerDocument document)
        {
            if (ServiceHost == ".")
                document.Host = string.Empty;
            else if (!string.IsNullOrEmpty(ServiceHost))
                document.Host = ServiceHost;

            if (string.IsNullOrEmpty(DocumentTemplate))
            {
                if (!string.IsNullOrEmpty(InfoTitle))
                    document.Info.Title = InfoTitle;
                if (!string.IsNullOrEmpty(InfoVersion))
                    document.Info.Version = InfoVersion;
                if (!string.IsNullOrEmpty(InfoDescription))
                    document.Info.Description = InfoDescription;
            }

            if (ServiceSchemes != null && ServiceSchemes.Any())
                document.Schemes = ServiceSchemes.Select(s => (SwaggerSchema)Enum.Parse(typeof(SwaggerSchema), s, true))
                    .ToList();

            if (!string.IsNullOrEmpty(ServiceBasePath))
                document.BasePath = ServiceBasePath;

            return document.ToJson(OutputType);
        }

        public async Task<string> GetDocumentTemplateAsync()
        {
            if (!string.IsNullOrEmpty(DocumentTemplate))
            {
                if (await DynamicApis.FileExistsAsync(DocumentTemplate).ConfigureAwait(false))
                {
                    var json = await DynamicApis.FileReadAllTextAsync(DocumentTemplate).ConfigureAwait(false);
                    if (json.StartsWith("{") == false)
                        return (await SwaggerYamlDocument.FromYamlAsync(json)).ToJson();
                    else
                        return json;
                }
                else if (Settings.DocumentTemplate.StartsWith("{") == false)
                    return (await SwaggerYamlDocument.FromYamlAsync(Settings.DocumentTemplate)).ToJson();
                else
                    return DocumentTemplate;
            }
            else
                return null;
        }
    }
}
