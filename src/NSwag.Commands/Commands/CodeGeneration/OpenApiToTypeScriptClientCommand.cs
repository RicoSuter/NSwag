//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptClientCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NConsole;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Infrastructure;
using NSwag.CodeGeneration.TypeScript;

#pragma warning disable 1591

namespace NSwag.Commands.CodeGeneration
{
    [Command(Name = "openapi2tsclient", Description = "Generates TypeScript client code from a Swagger/OpenAPI specification.")]
    public class OpenApiToTypeScriptClientCommand : SwaggerToTypeScriptClientCommand
    {
    }

    [Command(Name = "swagger2tsclient", Description = "Generates TypeScript client code from a Swagger/OpenAPI specification (obsolete: use openapi2tsclient instead).")]
    [Obsolete("Use openapi2tsclient instead.")]
    public class SwaggerToTypeScriptClientCommand : CodeGeneratorCommandBase<TypeScriptClientGeneratorSettings>
    {
        public SwaggerToTypeScriptClientCommand()
            : base(new TypeScriptClientGeneratorSettings())
        {
        }

        [Argument(Name = "ClassName", IsRequired = false, Description = "The class name of the generated client.")]
        public string ClassName
        {
            get { return Settings.ClassName; }
            set { Settings.ClassName = value; }
        }

        [Argument(Name = "ModuleName", IsRequired = false, Description = "The TypeScript module name (default: '', no module).")]
        public string ModuleName
        {
            get { return Settings.TypeScriptGeneratorSettings.ModuleName; }
            set { Settings.TypeScriptGeneratorSettings.ModuleName = value; }
        }

        [Argument(Name = "Namespace", IsRequired = false, Description = "The TypeScript namespace (default: '', no namespace).")]
        public string Namespace
        {
            get { return Settings.TypeScriptGeneratorSettings.Namespace; }
            set { Settings.TypeScriptGeneratorSettings.Namespace = value; }
        }

        [Argument(Name = "TypeScriptVersion", IsRequired = false, Description = "The target TypeScript version (default: 2.7).")]
        public decimal TypeScriptVersion
        {
            get { return Settings.TypeScriptGeneratorSettings.TypeScriptVersion; }
            set { Settings.TypeScriptGeneratorSettings.TypeScriptVersion = value; }
        }

        [Argument(Name = "Template", IsRequired = false, Description = "The type of the asynchronism handling " +
                                                                       "('JQueryCallbacks', 'JQueryPromises', 'AngularJS', 'Angular', 'Fetch', 'Aurelia').")]
        public TypeScriptTemplate Template
        {
            get { return Settings.Template; }
            set { Settings.Template = value; }
        }

        [Argument(Name = "PromiseType", IsRequired = false, Description = "The promise type ('Promise' or 'QPromise').")]
        public PromiseType PromiseType
        {
            get { return Settings.PromiseType; }
            set { Settings.PromiseType = value; }
        }

        [Argument(Name = "HttpClass", IsRequired = false, Description = "The Angular HTTP service class (default 'Http', 'HttpClient').")]
        public HttpClass HttpClass
        {
            get { return Settings.HttpClass; }
            set { Settings.HttpClass = value; }
        }

        [Argument(Name = "WithCredentials", IsRequired = false, Description = "The Angular HttpClient will send withCredentials: true in http requests (default: false).")]
        public bool WithCredentials
        {
            get { return Settings.WithCredentials; }
            set { Settings.WithCredentials = value; }
        }

        [Argument(Name = "UseSingletonProvider", IsRequired = false, Description = "Specifies whether to use the Angular 6 Singleton Provider (Angular template only, default: false).")]
        public bool UseSingletonProvider
        {
            get { return Settings.UseSingletonProvider; }
            set { Settings.UseSingletonProvider = value; }
        }

        [Argument(Name = "InjectionTokenType", IsRequired = false, Description = "The Angular injection token type (default 'InjectionToken', 'OpaqueToken').")]
        public InjectionTokenType InjectionTokenType
        {
            get { return Settings.InjectionTokenType; }
            set { Settings.InjectionTokenType = value; }
        }

        [Argument(Name = "RxJsVersion", IsRequired = false, Description = "The target RxJs version (default: 6.0).")]
        public decimal RxJsVersion
        {
            get { return Settings.RxJsVersion; }
            set { Settings.RxJsVersion = value; }
        }

        [Argument(Name = "DateTimeType", IsRequired = false, Description = "The date time type ('Date', 'MomentJS', 'Luxon', 'DayJS', 'OffsetMomentJS', 'string').")]
        public TypeScriptDateTimeType DateTimeType
        {
            get { return Settings.TypeScriptGeneratorSettings.DateTimeType; }
            set { Settings.TypeScriptGeneratorSettings.DateTimeType = value; }
        }

        [Argument(Name = "NullValue", IsRequired = false, Description = "The null value used in object initializers (default 'Undefined', 'Null').")]
        public TypeScriptNullValue NullValue
        {
            get { return Settings.TypeScriptGeneratorSettings.NullValue; }
            set { Settings.TypeScriptGeneratorSettings.NullValue = value; }
        }

        [Argument(Name = "GenerateClientClasses", IsRequired = false, Description = "Specifies whether generate client classes.")]
        public bool GenerateClientClasses
        {
            get { return Settings.GenerateClientClasses; }
            set { Settings.GenerateClientClasses = value; }
        }

        [Argument(Name = "GenerateClientInterfaces", IsRequired = false, Description = "Specifies whether generate interfaces for the client classes (default: false).")]
        public bool GenerateClientInterfaces
        {
            get { return Settings.GenerateClientInterfaces; }
            set { Settings.GenerateClientInterfaces = value; }
        }

        [Argument(Name = "GenerateOptionalParameters", IsRequired = false,
                  Description = "Specifies whether to reorder parameters (required first, optional at the end) and generate optional parameters (default: false).")]
        public bool GenerateOptionalParameters
        {
            get { return Settings.GenerateOptionalParameters; }
            set { Settings.GenerateOptionalParameters = value; }
        }

        [Argument(Name = "ExportTypes", IsRequired = false, Description = "Specifies whether the export keyword should be added to all classes, interfaces and enums (default: true).")]
        public bool ExportTypes
        {
            get { return Settings.TypeScriptGeneratorSettings.ExportTypes; }
            set { Settings.TypeScriptGeneratorSettings.ExportTypes = value; }
        }

        [Argument(Name = "WrapDtoExceptions", IsRequired = false, Description = "Specifies whether DTO exceptions are wrapped in a SwaggerException instance (default: false).")]
        public bool WrapDtoExceptions
        {
            get { return Settings.WrapDtoExceptions; }
            set { Settings.WrapDtoExceptions = value; }
        }

        [Argument(Name = "ExceptionClass", IsRequired = false, Description = "The exception class (default 'ApiException').")]
        public string ExceptionClass
        {
            get { return Settings.ExceptionClass; }
            set { Settings.ExceptionClass = value; }
        }

        [Argument(Name = "ClientBaseClass", IsRequired = false, Description = "The base class of the generated client classes (optional, must be imported or implemented in the extension code).")]
        public string ClientBaseClass
        {
            get { return Settings.ClientBaseClass; }
            set { Settings.ClientBaseClass = value; }
        }

        [Argument(Name = "WrapResponses", IsRequired = false, Description = "Specifies whether to wrap success responses to allow full response access (experimental).")]
        public bool WrapResponses
        {
            get { return Settings.WrapResponses; }
            set { Settings.WrapResponses = value; }
        }

        [Argument(Name = "WrapResponseMethods", IsRequired = false, Description = "List of methods where responses are wrapped ('ControllerName.MethodName', WrapResponses must be true).")]
        public string[] WrapResponseMethods
        {
            get { return Settings.WrapResponseMethods; }
            set { Settings.WrapResponseMethods = value; }
        }

        [Argument(Name = "GenerateResponseClasses", IsRequired = false, Description = "Specifies whether to generate response classes (default: true).")]
        public bool GenerateResponseClasses
        {
            get { return Settings.GenerateResponseClasses; }
            set { Settings.GenerateResponseClasses = value; }
        }

        [Argument(Name = "ResponseClass", IsRequired = false, Description = "The response class (default 'SwaggerResponse', may use '{controller}' placeholder).")]
        public string ResponseClass
        {
            get { return Settings.ResponseClass; }
            set { Settings.ResponseClass = value; }
        }

        [Argument(Name = "ProtectedMethods", IsRequired = false, Description = "List of methods with a protected access modifier ('classname.methodname').")]
        public string[] ProtectedMethods
        {
            get { return Settings.ProtectedMethods; }
            set { Settings.ProtectedMethods = value; }
        }

        [Argument(Name = "ConfigurationClass", IsRequired = false, Description = "The configuration class. The setting ClientBaseClass must be set. (empty for no configuration class).")]
        public string ConfigurationClass
        {
            get { return Settings.ConfigurationClass; }
            set { Settings.ConfigurationClass = value; }
        }

        [Argument(Name = "UseTransformOptionsMethod", IsRequired = false, Description = "Call 'transformOptions' on the base class or extension class (default: false).")]
        public bool UseTransformOptionsMethod
        {
            get { return Settings.UseTransformOptionsMethod; }
            set { Settings.UseTransformOptionsMethod = value; }
        }

        [Argument(Name = "UseTransformResultMethod", IsRequired = false, Description = "Call 'transformResult' on the base class or extension class (default: false).")]
        public bool UseTransformResultMethod
        {
            get { return Settings.UseTransformResultMethod; }
            set { Settings.UseTransformResultMethod = value; }
        }

        [Argument(Name = "GenerateDtoTypes", IsRequired = false, Description = "Specifies whether to generate DTO classes.")]
        public bool GenerateDtoTypes
        {
            get { return Settings.GenerateDtoTypes; }
            set { Settings.GenerateDtoTypes = value; }
        }

        [Argument(Name = "OperationGenerationMode", IsRequired = false, Description = "The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        public OperationGenerationMode OperationGenerationMode
        {
            get { return OperationGenerationModeConverter.GetOperationGenerationMode(Settings.OperationNameGenerator); }
            set { Settings.OperationNameGenerator = OperationGenerationModeConverter.GetOperationNameGenerator(value); }
        }

        [Argument(Name = "MarkOptionalProperties", IsRequired = false, Description = "Specifies whether to mark optional properties with ? (default: false).")]
        public bool MarkOptionalProperties
        {
            get { return Settings.TypeScriptGeneratorSettings.MarkOptionalProperties; }
            set { Settings.TypeScriptGeneratorSettings.MarkOptionalProperties = value; }
        }

        [Argument(Name = "GenerateCloneMethod", IsRequired = false, Description = "Specifies whether a clone() method should be generated in the DTO classes (default: false).")]
        public bool GenerateCloneMethod
        {
            get { return Settings.TypeScriptGeneratorSettings.GenerateCloneMethod; }
            set { Settings.TypeScriptGeneratorSettings.GenerateCloneMethod = value; }
        }

        [Argument(Name = "TypeStyle", IsRequired = false, Description = "The type style (default: Class).")]
        public TypeScriptTypeStyle TypeStyle
        {
            get { return Settings.TypeScriptGeneratorSettings.TypeStyle; }
            set { Settings.TypeScriptGeneratorSettings.TypeStyle = value; }
        }

        [Argument(Name = "EnumStyle", IsRequired = false, Description = "The enum style (Enum or StringLiteral, default: Enum).")]
        public TypeScriptEnumStyle EnumStyle
        {
            get { return Settings.TypeScriptGeneratorSettings.EnumStyle; }
            set { Settings.TypeScriptGeneratorSettings.EnumStyle = value; }
        }

        [Argument(Name = "UseLeafType", IsRequired = false, Description = "Generate leaf types for an object with discriminator (default: false).")]
        public bool UseLeafType
        {
            get { return Settings.TypeScriptGeneratorSettings.UseLeafType; }
            set { Settings.TypeScriptGeneratorSettings.UseLeafType = value; }
        }

        [Argument(Name = "ClassTypes", IsRequired = false, Description = "The type names which always generate plain TypeScript classes.")]
        public string[] ClassTypes
        {
            get { return Settings.TypeScriptGeneratorSettings.ClassTypes; }
            set { Settings.TypeScriptGeneratorSettings.ClassTypes = value; }
        }

        [Argument(Name = "ExtendedClasses", IsRequired = false, Description = "The list of extended classes.")]
        public string[] ExtendedClasses
        {
            get { return Settings.TypeScriptGeneratorSettings.ExtendedClasses; }
            set { Settings.TypeScriptGeneratorSettings.ExtendedClasses = value; }
        }

        [Argument(Name = "ExtensionCode", IsRequired = false, Description = "The extension code (string or file path).")]
        public string ExtensionCode { get; set; }

        [Argument(Name = "GenerateDefaultValues", IsRequired = false, Description = "Specifies whether to generate default values for properties (default: true).")]
        public bool GenerateDefaultValues
        {
            get { return Settings.TypeScriptGeneratorSettings.GenerateDefaultValues; }
            set { Settings.TypeScriptGeneratorSettings.GenerateDefaultValues = value; }
        }

        [Argument(Name = "ExcludedTypeNames", IsRequired = false, Description = "The excluded DTO type names (must be defined in an import or other namespace).")]
        public string[] ExcludedTypeNames
        {
            get { return Settings.TypeScriptGeneratorSettings.ExcludedTypeNames; }
            set { Settings.TypeScriptGeneratorSettings.ExcludedTypeNames = value; }
        }

        [Argument(Name = "ExcludedParameterNames", IsRequired = false, Description = "The globally excluded parameter names.")]
        public string[] ExcludedParameterNames
        {
            get { return Settings.ExcludedParameterNames; }
            set { Settings.ExcludedParameterNames = value; }
        }

        [Argument(Name = "HandleReferences", IsRequired = false, Description = "Handle JSON references (default: false).")]
        public bool HandleReferences
        {
            get { return Settings.TypeScriptGeneratorSettings.HandleReferences; }
            set { Settings.TypeScriptGeneratorSettings.HandleReferences = value; }
        }

        [Argument(Name = "GenerateTypeCheckFunctions", IsRequired = false, Description = "Generate type check functions (only available when TypeStyle is Interface, default: false).")]
        public bool GenerateTypeCheckFunctions
        {
            get { return Settings.TypeScriptGeneratorSettings.GenerateTypeCheckFunctions; }
            set { Settings.TypeScriptGeneratorSettings.GenerateTypeCheckFunctions = value; }
        }

        [Argument(Name = "GenerateConstructorInterface", IsRequired = false, Description = "Generate an class interface which is used in the constructor to initialize the class (only available when TypeStyle is Class, default: true).")]
        public bool GenerateConstructorInterface
        {
            get { return Settings.TypeScriptGeneratorSettings.GenerateConstructorInterface; }
            set { Settings.TypeScriptGeneratorSettings.GenerateConstructorInterface = value; }
        }

        [Argument(Name = "ConvertConstructorInterfaceData", IsRequired = false, Description = "Convert POJO objects in the constructor data to DTO instances (GenerateConstructorInterface must be enabled, default: false).")]
        public bool ConvertConstructorInterfaceData
        {
            get { return Settings.TypeScriptGeneratorSettings.ConvertConstructorInterfaceData; }
            set { Settings.TypeScriptGeneratorSettings.ConvertConstructorInterfaceData = value; }
        }

        [Argument(Name = "ImportRequiredTypes", IsRequired = false, Description = "Specifies whether required types should be imported (default: true).")]
        public bool ImportRequiredTypes
        {
            get { return Settings.ImportRequiredTypes; }
            set { Settings.ImportRequiredTypes = value; }
        }

        [Argument(Name = "UseGetBaseUrlMethod", IsRequired = false, Description = "Specifies whether to use the 'getBaseUrl(defaultUrl: string)' method from the base class (default: false).")]
        public bool UseGetBaseUrlMethod
        {
            get { return Settings.UseGetBaseUrlMethod; }
            set { Settings.UseGetBaseUrlMethod = value; }
        }

        [Argument(Name = "BaseUrlTokenName", IsRequired = false, Description = "The token name for injecting the API base URL string (used in the Angular template, default: 'API_BASE_URL').")]
        public string BaseUrlTokenName
        {
            get { return Settings.BaseUrlTokenName; }
            set { Settings.BaseUrlTokenName = value; }
        }

        [Argument(Name = nameof(QueryNullValue), IsRequired = false,
            Description = "The null value used for query parameters which are null (default: '').")]
        public string QueryNullValue
        {
            get { return Settings.QueryNullValue; }
            set { Settings.QueryNullValue = value; }
        }

        [Argument(Name = "UseAbortSignal", IsRequired = false, Description = "Specifies whether to use the AbortSignal (Fetch/Aurelia template only, default: false).")]
        public bool UseAbortSignal
        {
            get { return Settings.UseAbortSignal; }
            set { Settings.UseAbortSignal = value; }
        }

        [Argument(Name = "InlineNamedDictionaries", Description = "Inline named dictionaries (default: false).", IsRequired = false)]
        public bool InlineNamedDictionaries
        {
            get { return Settings.TypeScriptGeneratorSettings.InlineNamedDictionaries; }
            set { Settings.TypeScriptGeneratorSettings.InlineNamedDictionaries = value; }
        }

        [Argument(Name = "InlineNamedAny", Description = "Inline named any types (default: false).", IsRequired = false)]
        public bool InlineNamedAny
        {
            get { return Settings.TypeScriptGeneratorSettings.InlineNamedAny; }
            set { Settings.TypeScriptGeneratorSettings.InlineNamedAny = value; }
        }

        [Argument(Name = "IncludeHttpContext", IsRequired = false, Description = "Gets a value indicating whether to include the httpContext (Angular template only, default: false).")]
        public bool IncludeHttpContext
        {
            get { return Settings.IncludeHttpContext; }
            set { Settings.IncludeHttpContext = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var code = await RunAsync();
            await TryWriteFileOutputAsync(host, () => code).ConfigureAwait(false);
            return code;
        }

        public async Task<string> RunAsync()
        {
            var additionalCode = ExtensionCode ?? string.Empty;
            if (DynamicApis.FileExists(additionalCode))
            {
                additionalCode = DynamicApis.FileReadAllText(additionalCode);
            }

            Settings.TypeScriptGeneratorSettings.ExtensionCode = additionalCode;

            var document = await GetInputSwaggerDocument().ConfigureAwait(false);
            var clientGenerator = new TypeScriptClientGenerator(document, Settings);
            return clientGenerator.GenerateFile();
        }
    }
}
