//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpControllerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToCSharpControllerGenerator : SwaggerToCSharpGeneratorBase
    {
        private readonly SwaggerDocument _document;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpControllerGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpControllerGenerator(SwaggerDocument document, SwaggerToCSharpControllerGeneratorSettings settings)
            : this(document, settings, CreateResolverWithExceptionSchema(settings.CSharpGeneratorSettings, document))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpControllerGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpControllerGenerator(SwaggerDocument document, SwaggerToCSharpControllerGeneratorSettings settings, CSharpTypeResolver resolver)
            : base(document, settings, resolver)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Settings = settings;
            _document = document;
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToCSharpControllerGeneratorSettings Settings { get; set; }

        /// <summary>Gets the base settings.</summary>
        public override ClientGeneratorBaseSettings BaseSettings => Settings;

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public override string GenerateFile()
        {
            return GenerateFile(_document, ClientGeneratorOutputType.Full);
        }

        /// <summary>Generates the client class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerClassName">Name of the controller class.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <returns>The code.</returns>
        protected override string GenerateClientClass(string controllerName, string controllerClassName, IList<CSharpOperationModel> operations, ClientGeneratorOutputType outputType)
        {
            var model = new CSharpControllerTemplateModel(controllerClassName, operations, _document, Settings);
            var template = Settings.CodeGeneratorSettings.TemplateFactory.CreateTemplate("CSharp", "Controller", model);
            return template.Render();
        }

        /// <summary>Creates an operation model.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The operation model.</returns>
        protected override CSharpOperationModel CreateOperationModel(SwaggerOperation operation, ClientGeneratorBaseSettings settings)
        {
            return new CSharpOperationModel(operation, (SwaggerToCSharpGeneratorSettings)settings, this, (CSharpTypeResolver)Resolver);
        }
    }
}
