//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpClientGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using NSwag.CodeGeneration.CodeGenerators.CSharp.Models;
using NSwag.CodeGeneration.CodeGenerators.CSharp.Templates;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToCSharpClientGenerator : SwaggerToCSharpGeneratorBase
    {
        private readonly SwaggerService _service;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpClientGenerator" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">service</exception>
        /// <exception cref="ArgumentNullException"><paramref name="service" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpClientGenerator(SwaggerService service, SwaggerToCSharpClientGeneratorSettings settings) 
            : base(service, settings.CSharpGeneratorSettings)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            Settings = settings; 

            _service = service;
            foreach (var definition in _service.Definitions)
                definition.Value.TypeName = definition.Key;

        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToCSharpClientGeneratorSettings Settings { get; set; }

        /// <summary>Gets the language.</summary>
        protected override string Language => "CSharp";

        internal override ClientGeneratorBaseSettings BaseSettings => Settings;

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public override string GenerateFile()
        {
            return GenerateFile(_service, Resolver);
        }

        internal override string RenderFile(string clientCode, string[] clientClasses)
        {
            var template = new FileTemplate();
            template.Initialize(new
            {
                Namespace = Settings.CSharpGeneratorSettings.Namespace ?? string.Empty, 
                Toolchain = SwaggerService.ToolchainVersion, 
                Clients = Settings.GenerateClientClasses ? clientCode : string.Empty,
                NamespaceUsages = Settings.AdditionalNamespaceUsages ?? new string[] { },
                Classes = Settings.GenerateDtoTypes ? Resolver.GenerateTypes() : string.Empty
            });
            return template.Render();
        }

        internal override string RenderClientCode(string controllerName, IList<OperationModel> operations)
        {
            var template = new ClientTemplate();
            template.Initialize(new ClientTemplateModel(controllerName, operations, _service, Settings));
            return template.Render();
        }
    }
}
