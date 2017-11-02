//using NConsole;
//using System.Threading.Tasks;
//using NJsonSchema;
//using Microsoft.AspNetCore.Mvc.ApiExplorer;
//using Microsoft.Extensions.DependencyInjection;
//using NSwag.SwaggerGeneration.AspNetCore;
//using System.IO;

//namespace NSwag.Commands.AspNetCore
//{
//    internal class AspNetToSwaggerInnerCommand : IConsoleCommand
//    {
//        public AspNetCoreToSwaggerGeneratorCommandSettings Settings { get; } = new AspNetCoreToSwaggerGeneratorCommandSettings();

//        [Argument(Name = "ApplicationName", IsRequired = true, Description = "The application name.")]
//        public string ApplicationName { get; set; }

//        [Argument(Name = "OutputPath", IsRequired = true, Description = "The output path.")]
//        public string OutputPath { get; set; }

//        [Argument(Name = "DefaultPropertyNameHandling", IsRequired = false, Description = "The default property name handling ('Default' or 'CamelCase').")]
//        public PropertyNameHandling DefaultPropertyNameHandling
//        {
//            get => Settings.DefaultPropertyNameHandling;
//            set => Settings.DefaultPropertyNameHandling = value;
//        }

//        [Argument(Name = "DefaultReferenceTypeNullHandling", IsRequired = false, Description = "The default null handling (if NotNullAttribute and CanBeNullAttribute are missing, default: Null, Null or NotNull).")]
//        public ReferenceTypeNullHandling DefaultReferenceTypeNullHandling
//        {
//            get => Settings.DefaultReferenceTypeNullHandling;
//            set => Settings.DefaultReferenceTypeNullHandling = value;
//        }

//        [Argument(Name = "DefaultEnumHandling", IsRequired = false, Description = "The default enum handling ('String' or 'Integer'), default: Integer.")]
//        public EnumHandling DefaultEnumHandling
//        {
//            get => Settings.DefaultEnumHandling;
//            set => Settings.DefaultEnumHandling = value;
//        }

//        [Argument(Name = "FlattenInheritanceHierarchy", IsRequired = false, Description = "Flatten the inheritance hierarchy instead of using allOf to describe inheritance (default: false).")]
//        public bool FlattenInheritanceHierarchy
//        {
//            get => Settings.FlattenInheritanceHierarchy;
//            set => Settings.FlattenInheritanceHierarchy = value;
//        }

//        [Argument(Name = "GenerateKnownTypes", IsRequired = false, Description = "Generate schemas for types in KnownTypeAttribute attributes (default: true).")]
//        public bool GenerateKnownTypes
//        {
//            get => Settings.GenerateKnownTypes;
//            set => Settings.GenerateKnownTypes = value;
//        }

//        [Argument(Name = "GenerateXmlObjects", IsRequired = false, Description = "Generate xmlObject representation for definitions (default: false).")]
//        public bool GenerateXmlObjects
//        {
//            get => Settings.GenerateXmlObjects;
//            set => Settings.GenerateXmlObjects = value;
//        }

//        [Argument(Name = "GenerateAbstractProperties", IsRequired = false, Description = "Generate abstract properties (i.e. interface and abstract properties. Properties may defined multiple times in a inheritance hierarchy, default: false).")]
//        public bool GenerateAbstractProperties
//        {
//            get => Settings.GenerateAbstractProperties;
//            set => Settings.GenerateAbstractProperties = value;
//        }

//        [Argument(Name = "ServiceHost", IsRequired = false, Description = "Overrides the service host of the web service (optional, use '.' to remove the hostname).")]
//        public string ServiceHost { get; set; }

//        [Argument(Name = "ServiceBasePath", IsRequired = false, Description = "The basePath of the Swagger specification (optional).")]
//        public string ServiceBasePath { get; set; }

//        [Argument(Name = "ServiceSchemes", IsRequired = false, Description = "Overrides the allowed schemes of the web service (optional, comma separated, 'http', 'https', 'ws', 'wss').")]
//        public string[] ServiceSchemes { get; set; }

//        [Argument(Name = "InfoTitle", IsRequired = false, Description = "Specify the title of the Swagger specification.")]
//        public string InfoTitle
//        {
//            get => Settings.Title;
//            set => Settings.Title = value;
//        }

//        [Argument(Name = "InfoDescription", IsRequired = false, Description = "Specify the description of the Swagger specification.")]
//        public string InfoDescription
//        {
//            get => Settings.Description;
//            set => Settings.Description = value;
//        }

//        [Argument(Name = "InfoVersion", IsRequired = false, Description = "Specify the version of the Swagger specification (default: 1.0.0).")]
//        public string InfoVersion
//        {
//            get => Settings.Version;
//            set => Settings.Version = value;
//        }

//        [Argument(Name = "DocumentTemplate", IsRequired = false, Description = "Specifies the Swagger document template (may be a path or JSON, default: none).")]
//        public string DocumentTemplate { get; set; }

//        [Argument(Name = "DocumentProcessors", IsRequired = false, Description = "Gets the document processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
//        public string[] DocumentProcessorTypes
//        {
//            get => Settings.DocumentProcessorTypes;
//            set => Settings.DocumentProcessorTypes = value;
//        }

//        [Argument(Name = "OperationProcessors", IsRequired = false, Description = "Gets the operation processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
//        public string[] OperationProcessorTypes
//        {
//            get => Settings.OperationProcessorTypes;
//            set => Settings.OperationProcessorTypes = value;
//        }

//        public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
//        {
//            var serviceProvider = ApplicationServiceProvider.GetServiceProvider(ApplicationName);
//            var apiDescriptionProvider = serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

//            var swaggerGenerator = new AspNetCoreToSwaggerGenerator(Settings);
//            var swaggerDocument = await swaggerGenerator.GenerateAsync(apiDescriptionProvider.ApiDescriptionGroups);
//            File.WriteAllText(OutputPath, swaggerDocument.ToJson());

//            return swaggerDocument;
//        }
//    }
//}
