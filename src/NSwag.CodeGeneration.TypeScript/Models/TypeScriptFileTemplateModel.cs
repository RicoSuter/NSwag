//-----------------------------------------------------------------------
// <copyright file="ClientTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>The TypeScript file template model.</summary>
    public class TypeScriptFileTemplateModel
    {
        private readonly SwaggerToTypeScriptClientGeneratorSettings _settings;
        private readonly TypeScriptTypeResolver _resolver;
        private readonly string _clientCode;
        private readonly SwaggerDocument _document;
        private readonly TypeScriptExtensionCode _extensionCode;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptFileTemplateModel" /> class.</summary>
        /// <param name="clientCode">The client code.</param>
        /// <param name="clientClasses">The client classes.</param>
        /// <param name="document">The Swagger document.</param>
        /// <param name="extensionCode">The extension code.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        public TypeScriptFileTemplateModel(
            string clientCode,
            IEnumerable<string> clientClasses,
            SwaggerDocument document,
            TypeScriptExtensionCode extensionCode,
            SwaggerToTypeScriptClientGeneratorSettings settings,
            TypeScriptTypeResolver resolver)
        {
            _document = document;
            _extensionCode = extensionCode;
            _settings = settings;
            _resolver = resolver;
            _clientCode = clientCode;
            ClientClasses = clientClasses.ToArray();

            Types = GenerateDtoTypes();
            ExtensionCodeBottom = GenerateExtensionCodeAfter();
        }

        /// <summary>Gets a value indicating whether to generate client classes.</summary>
        public bool GenerateClientClasses => _settings.GenerateClientClasses;

        /// <summary>Gets a value indicating whether the generated code is for Angular 2.</summary>
        public bool IsAngular => _settings.Template == TypeScriptTemplate.Angular;

        /// <summary>Gets a value indicating whether to use HttpClient with the Angular template.</summary>
        public bool UseAngularHttpClient => _settings.HttpClass == HttpClass.HttpClient;

        /// <summary>Gets a value indicating whether the generated code is for Aurelia.</summary>
        public bool IsAurelia => _settings.Template == TypeScriptTemplate.Aurelia;

        /// <summary>Gets a value indicating whether the generated code is for Angular.</summary>
        public bool IsAngularJS => _settings.Template == TypeScriptTemplate.AngularJS;

        /// <summary>Gets a value indicating whether the generated code is for Knockout.</summary>
        public bool IsKnockout => _settings.TypeScriptGeneratorSettings.TypeStyle == TypeScriptTypeStyle.KnockoutClass;

        /// <summary>Gets a value indicating whether to render for JQuery.</summary>
        public bool IsJQuery => _settings.Template == TypeScriptTemplate.JQueryCallbacks || _settings.Template == TypeScriptTemplate.JQueryPromises;

        /// <summary>Gets or sets a value indicating whether DTO exceptions are wrapped in a SwaggerException instance.</summary>
        public bool WrapDtoExceptions => _settings.WrapDtoExceptions;

        /// <summary>Gets or sets a value indicating whether to wrap success responses to allow full response access.</summary>
        public bool WrapResponses => _settings.WrapResponses;

        /// <summary>Gets or sets a value indicating whether to generate the response class (only applied when WrapResponses == true, default: true).</summary>
        public bool GenerateResponseClasses => _settings.GenerateResponseClasses;

        /// <summary>Gets the response class names.</summary>
        public IEnumerable<string> ResponseClassNames
        {
            get
            {
                // TODO: Merge with ResponseClassNames of C#
                if (_settings.OperationNameGenerator.SupportsMultipleClients)
                {
                    return _document.Operations
                        .GroupBy(o => _settings.OperationNameGenerator.GetClientName(_document, o.Path, o.Method, o.Operation))
                        .Select(g => _settings.ResponseClass.Replace("{controller}", g.Key))
                        .Where(a => _settings.TypeScriptGeneratorSettings.ExcludedTypeNames?.Contains(a) != true)
                        .Distinct();
                }

                return new[] { _settings.ResponseClass.Replace("{controller}", string.Empty) };
            }
        }

        /// <summary>Gets a value indicating whether MomentJS is required.</summary>
        public bool RequiresMomentJS => _settings.TypeScriptGeneratorSettings.DateTimeType == TypeScriptDateTimeType.MomentJS;

        /// <summary>Gets a value indicating whether required types should be imported.</summary>
        public bool ImportRequiredTypes => _settings.ImportRequiredTypes;

        /// <summary>Gets a value indicating whether to call 'transformOptions' on the base class or extension class.</summary>
        public bool UseTransformOptionsMethod => _settings.UseTransformOptionsMethod;

        /// <summary>Gets the clients code.</summary>
        public string Clients => _settings.GenerateClientClasses ? _clientCode : string.Empty;

        /// <summary>Gets the types code.</summary>
        public string Types { get; }

        /// <summary>Gets or sets the extension code imports.</summary>
        public string ExtensionCodeImport => _extensionCode.ImportCode;

        /// <summary>Gets or sets the extension code to insert at the beginning.</summary>
        public string ExtensionCodeTop => _settings.ConfigurationClass != null && _extensionCode.ExtensionClasses.ContainsKey(_settings.ConfigurationClass) ?
            _extensionCode.ExtensionClasses[_settings.ConfigurationClass] + "\n\n" + _extensionCode.TopCode :
            _extensionCode.TopCode;

        /// <summary>Gets or sets the extension code to insert at the end.</summary>
        public string ExtensionCodeBottom { get; }

        /// <summary>Gets a value indicating whether the file has module name.</summary>
        public bool HasModuleName => !string.IsNullOrEmpty(_settings.TypeScriptGeneratorSettings.ModuleName);

        /// <summary>Gets the name of the module.</summary>
        public string ModuleName => _settings.TypeScriptGeneratorSettings.ModuleName;

        /// <summary>Gets a value indicating whether the file has a namespace.</summary>
        public bool HasNamespace => !string.IsNullOrEmpty(_settings.TypeScriptGeneratorSettings.Namespace);

        /// <summary>Gets the namespace.</summary>
        public string Namespace => _settings.TypeScriptGeneratorSettings.Namespace;

        /// <summary>Gets a value indicating whether the FileParameter interface should be rendered.</summary>
        public bool RequiresFileParameterInterface =>
            !_settings.TypeScriptGeneratorSettings.ExcludedTypeNames.Contains("FileParameter") &&
            _document.Operations.Any(o => o.Operation.Parameters.Any(p => p.Type.HasFlag(JsonObjectType.File)));

        /// <summary>Gets a value indicating whether the FileResponse interface should be rendered.</summary>
        public bool RequiresFileResponseInterface =>
            !IsJQuery &&
            !_settings.TypeScriptGeneratorSettings.ExcludedTypeNames.Contains("FileResponse") &&
            _document.Operations.Any(o => o.Operation.ActualResponses.Any(r => r.Value.Schema?.ActualSchema.Type == JsonObjectType.File));

        /// <summary>Gets a value indicating whether the SwaggerException class is required.</summary>
        public bool RequiresSwaggerExceptionClass =>
            !_settings.TypeScriptGeneratorSettings.ExcludedTypeNames.Contains("SwaggerException") &&
            _settings.GenerateClientClasses &&
            !string.IsNullOrEmpty(Clients);

        /// <summary>Table containing list of the generated classes.</summary>
        public string[] ClientClasses { get; }

        /// <summary>Gets a value indicating whether to handle references.</summary>
        public bool HandleReferences => _settings.TypeScriptGeneratorSettings.HandleReferences;

        /// <summary>Gets the reference handling code.</summary>
        public string ReferenceHandlingCode => TypeScriptReferenceHandlingCodeGenerator.Generate();

        // Angular only

        /// <summary>Gets or sets the injection token type (used in the Angular template).</summary>
        public string InjectionTokenType => _settings.InjectionTokenType.ToString();

        /// <summary>Gets or sets the token name for injecting the API base URL string (used in the Angular template).</summary>
        public string BaseUrlTokenName => _settings.BaseUrlTokenName;

        private string GenerateDtoTypes()
        {
            return _settings.GenerateDtoTypes ? _resolver.GenerateTypes(_extensionCode).Concatenate() : string.Empty;
        }

        private string GenerateExtensionCodeAfter()
        {
            var clientClassesVariable = "{" + string.Join(", ", ClientClasses.Select(c => "'" + c + "': " + c)) + "}";
            return _extensionCode.BottomCode.Replace("{clientClasses}", clientClassesVariable);
        }
    }
}