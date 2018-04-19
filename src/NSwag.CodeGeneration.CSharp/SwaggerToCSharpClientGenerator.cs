//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpClientGenerator.cs" company="NSwag">
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
    public class SwaggerToCSharpClientGenerator : SwaggerToCSharpGeneratorBase
    {
        private readonly SwaggerDocument _document;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpClientGenerator(SwaggerDocument document, SwaggerToCSharpClientGeneratorSettings settings)
            : this(document, settings, CreateResolverWithExceptionSchema(settings.CSharpGeneratorSettings, document))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpClientGenerator(SwaggerDocument document, SwaggerToCSharpClientGeneratorSettings settings, CSharpTypeResolver resolver)
            : base(document, settings, resolver)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Settings = settings;
            _document = document;
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToCSharpClientGeneratorSettings Settings { get; }

        /// <summary>Gets the base settings.</summary>
        public override ClientGeneratorBaseSettings BaseSettings => Settings;

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public override string GenerateFile()
        {
            return GenerateFile(ClientGeneratorOutputType.Full);
        }

        /// <summary>Generates the the whole file containing all needed types.</summary>
        /// <param name="outputType">The output type.</param>
        /// <returns>The code</returns>
        public string GenerateFile(ClientGeneratorOutputType outputType)
        {
            return GenerateFile(_document, outputType);
        }

        /// <summary>Generates the client class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerClassName">Name of the controller class.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <returns>The code.</returns>
        protected override string GenerateClientClass(string controllerName, string controllerClassName, IList<CSharpOperationModel> operations, ClientGeneratorOutputType outputType)
        {
            var exceptionSchema = (Resolver as CSharpTypeResolver)?.ExceptionSchema;
            var model = new CSharpClientTemplateModel(controllerName, controllerClassName, operations, exceptionSchema, _document, Settings)
            {
                GenerateContracts = outputType == ClientGeneratorOutputType.Full || outputType == ClientGeneratorOutputType.Contracts,
                GenerateImplementation = outputType == ClientGeneratorOutputType.Full || outputType == ClientGeneratorOutputType.Implementation,
            };

            var template = Settings.CSharpGeneratorSettings.TemplateFactory.CreateTemplate("CSharp", "Client", model);
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
