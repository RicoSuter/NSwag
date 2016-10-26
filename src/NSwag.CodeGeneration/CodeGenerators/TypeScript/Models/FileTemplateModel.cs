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

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript.Models
{
    /// <summary>The TypeScript file template model.</summary>
    public class FileTemplateModel
    {
        private readonly SwaggerToTypeScriptClientGeneratorSettings _settings;
        private readonly TypeScriptTypeResolver _resolver;
        private readonly string _clientCode;
        private readonly SwaggerService _service;

        /// <summary>Initializes a new instance of the <see cref="FileTemplateModel" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="clientCode">The client code.</param>
        /// <param name="clientClasses">The client classes.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        public FileTemplateModel(SwaggerService service, string clientCode, IEnumerable<string> clientClasses, 
            SwaggerToTypeScriptClientGeneratorSettings settings, TypeScriptTypeResolver resolver)
        {
            _service = service; 
            _settings = settings;
            _resolver = resolver;
            _clientCode = clientCode;
            ClientClasses = clientClasses.ToArray();

            Types = GenerateDtoTypes();
            ExtensionCodeAfter = GenerateExtensionCodeAfter();
        }

        /// <summary>Gets a value indicating whether the generated code is for Angular 2.</summary>
        public bool IsAngular2 => _settings.GenerateClientClasses && _settings.Template == TypeScriptTemplate.Angular2;

        /// <summary>Gets a value indicating whether the generated code is for Aurelia.</summary>
        public bool IsAurelia => _settings.GenerateClientClasses && _settings.Template == TypeScriptTemplate.Aurelia;

        /// <summary>Gets the clients code.</summary>
        public string Clients => _settings.GenerateClientClasses ? _clientCode : string.Empty;

        /// <summary>Gets the types code.</summary>
        public string Types { get; }

        /// <summary>Gets or sets the extension code to insert at the beginning.</summary>
        public string ExtensionCodeBefore => _settings.TypeScriptGeneratorSettings.ProcessedExtensionCode.CodeBefore;

        /// <summary>Gets or sets the extension code to insert at the end.</summary>
        public string ExtensionCodeAfter { get; }

        /// <summary>Gets a value indicating whether the file has module name.</summary>
        public bool HasModuleName => !string.IsNullOrEmpty(_settings.TypeScriptGeneratorSettings.ModuleName);

        /// <summary>Gets the name of the module.</summary>
        public string ModuleName => _settings.TypeScriptGeneratorSettings.ModuleName;

        /// <summary>Gets a value indicating whether the file has a namespace.</summary>
        public bool HasNamespace => !string.IsNullOrEmpty(_settings.TypeScriptGeneratorSettings.Namespace);

        /// <summary>Gets the namespace.</summary>
        public string Namespace => _settings.TypeScriptGeneratorSettings.Namespace;

        /// <summary>Gets a value indicating whether the FileParameter interface should be rendered.</summary>
        public bool RequiresFileParameterInterface => _service.Operations.Any(o => o.Operation.Parameters.Any(p => p.Type.HasFlag(JsonObjectType.File)));

        /// <summary>Table containing list of the generated classes.</summary>
        public string[] ClientClasses { get; }

        private string GenerateDtoTypes()
        {
            return _settings.GenerateDtoTypes ? _resolver.GenerateTypes(_settings.TypeScriptGeneratorSettings.ProcessedExtensionCode) : string.Empty;
        }

        private string GenerateExtensionCodeAfter()
        {
            var clientClassesVariable = "{" + string.Join(", ", ClientClasses.Select(c => "'" + c + "': " + c)) + "}";
            return _settings.TypeScriptGeneratorSettings.ProcessedExtensionCode.CodeAfter.Replace("{clientClasses}", clientClassesVariable);
        }
    }
}