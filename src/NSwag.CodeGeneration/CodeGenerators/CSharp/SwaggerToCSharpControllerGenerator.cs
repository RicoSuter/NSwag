//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToCSharpControllerGenerator : SwaggerToCSharpGenerator
    {
        private readonly SwaggerService _service;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpControllerGenerator" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">service</exception>
        /// <exception cref="ArgumentNullException"><paramref name="service" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpControllerGenerator(SwaggerService service, SwaggerToCSharpControllerGeneratorSettings settings) 
            : base(service, settings.CSharpGeneratorSettings)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            Settings = settings; 

            _service = service;
            foreach (var definition in _service.Definitions)
                definition.Value.TypeName = definition.Key;
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToCSharpControllerGeneratorSettings Settings { get; set; }

        /// <summary>Gets the language.</summary>
        protected override string Language
        {
            get { return "CSharp"; }
        }

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public override string GenerateFile()
        {
            return GenerateFile(_service, Resolver);
        }

        internal override CodeGeneratorBaseSettings BaseSettings
        {
            get { return Settings; }
        }

        internal override string RenderFile(string clientCode)
        {
            var template = LoadTemplate("File");
            template.Add("namespace", Settings.CSharpGeneratorSettings.Namespace);
            template.Add("toolchain", SwaggerService.ToolchainVersion);
            template.Add("clients", Settings.GenerateClientClasses ? clientCode : string.Empty);
            template.Add("namespaceUsages", Settings.AdditionalNamespaceUsages ?? new string[] {});
            template.Add("classes", Settings.GenerateDtoTypes ? Resolver.GenerateTypes() : string.Empty);
            return template.Render();
        }

        internal override string RenderClientCode(string controllerName, IEnumerable<OperationModel> operations)
        {
            var template = LoadTemplate("Controller");
            template.Add("class", Settings.ClassName.Replace("{controller}", ConvertToUpperStartIdentifier(controllerName)));

            var hasClientBaseClass = !string.IsNullOrEmpty(Settings.ControllerBaseClass); 
            template.Add("clientBaseClass", Settings.ControllerBaseClass);
            template.Add("hasClientBaseClass", hasClientBaseClass);

            template.Add("baseUrl", _service.BaseUrl);
            template.Add("operations", operations);
            template.Add("hasOperations", operations.Any());

            return template.Render();
        }
    }
}
