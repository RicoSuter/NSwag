//-----------------------------------------------------------------------
// <copyright file="ClientTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema.CodeGeneration.TypeScript;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript.Models
{
    /// <summary>The Typescript file template model.</summary>
    public class FileTemplateModel
    {
        private readonly SwaggerToTypeScriptClientGeneratorSettings _settings;
        private readonly TypeScriptTypeResolver _resolver;
        private readonly string _clientCode;

        /// <summary>Gets a value indicating whether this instance is angular2.</summary>
        public bool IsAngular2 => _settings.GenerateClientClasses && _settings.Template == TypeScriptTemplate.Angular2;

        /// <summary>Gets the clients.</summary>
        public string Clients =>_settings.GenerateClientClasses ? _clientCode : string.Empty;

        /// <summary>Gets the types.</summary>
        public string Types { get; }

        /// <summary>Gets the extension code before rendering.</summary>
        public string ExtensionCodeBefore => _settings.TypeScriptGeneratorSettings.ProcessedExtensionCode.CodeBefore;

        /// <summary>Gets the extension code after rendering.</summary>
        public string ExtensionCodeAfter { get; }

        /// <summary>Gets a value indicating whether this instance has module name.</summary>
        public bool HasModuleName => !string.IsNullOrEmpty(_settings.TypeScriptGeneratorSettings.ModuleName);

        /// <summary>Gets the name of the module.</summary>
        public string ModuleName => _settings.TypeScriptGeneratorSettings.ModuleName;

        /// <summary>Gets a value indicating whether this instance has namespace.</summary>
        public bool HasNamespace => !string.IsNullOrEmpty(_settings.TypeScriptGeneratorSettings.Namespace);

        /// <summary>Gets the namespace.</summary>
        public string Namespace => _settings.TypeScriptGeneratorSettings.Namespace;

        /// <summary>Initializes a new instance of the <see cref="FileTemplateModel"/> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="clientCode">The client code.</param>
        /// <param name="clientClasses">The client classes.</param>
        public FileTemplateModel(
           SwaggerToTypeScriptClientGeneratorSettings settings,
           TypeScriptTypeResolver resolver,
           string clientCode,
           IEnumerable<string> clientClasses
            )
        {
            _settings = settings;
            _resolver = resolver;
            _clientCode = clientCode;

            Types = GenerateDtoTypes();
            ExtensionCodeAfter = GenerateExtensionCodeAfter(clientClasses);
        }

        private string GenerateDtoTypes()
        {
            return _settings.GenerateDtoTypes ? _resolver.GenerateTypes(_settings.TypeScriptGeneratorSettings.ProcessedExtensionCode) : string.Empty;
        }

        private string GenerateExtensionCodeAfter(IEnumerable<string> clientClasses)
        {
            var clientClassesVariable = "{" + string.Join(", ", clientClasses.Select(c => "'" + c + "': " + c)) + "}";
            return _settings.TypeScriptGeneratorSettings.ProcessedExtensionCode.CodeAfter.Replace("{clientClasses}", clientClassesVariable);
        }
    }
}