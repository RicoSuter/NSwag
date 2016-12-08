//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpWebApiControllerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NSwag.CodeGeneration.CodeGenerators.CSharp.Models;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToCSharpWebApiControllerGenerator : SwaggerToCSharpGeneratorBase
    {
        private readonly SwaggerDocument _document;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpWebApiControllerGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpWebApiControllerGenerator(SwaggerDocument document, SwaggerToCSharpWebApiControllerGeneratorSettings settings)
            : this(document, settings, SwaggerToCSharpTypeResolver.CreateWithDefinitions(settings.CSharpGeneratorSettings, document.Definitions))
        {

        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpWebApiControllerGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpWebApiControllerGenerator(SwaggerDocument document, SwaggerToCSharpWebApiControllerGeneratorSettings settings, SwaggerToCSharpTypeResolver resolver)
            : base(document, settings, resolver)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Settings = settings;
            _document = document;
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToCSharpWebApiControllerGeneratorSettings Settings { get; set; }

        internal override ClientGeneratorBaseSettings BaseSettings => Settings;

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public override string GenerateFile()
        {
            return GenerateFile(_document, ClientGeneratorOutputType.Full);
        }

        internal override string GenerateClientClass(string controllerName, string controllerClassName, IList<OperationModel> operations, ClientGeneratorOutputType outputType)
        {
            var model = new ControllerTemplateModel(Settings)
            {
                Class = controllerClassName,
                BaseUrl = _document.BaseUrl,
                Operations = operations
            };

            var template = Settings.CodeGeneratorSettings.TemplateFactory.CreateTemplate("CSharp", "Controller", model);
            return template.Render();
        }
    }
}
