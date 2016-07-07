//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpClientGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
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
            : base(service, settings)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            Settings = settings;

            _service = service;
            foreach (var definition in _service.Definitions.Where(p => string.IsNullOrEmpty(p.Value.TypeNameRaw)))
                definition.Value.TypeNameRaw = definition.Key;
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
            return GenerateFile(ClientGeneratorOutputType.Full);
        }

        /// <summary>Generates the the whole file containing all needed types.</summary>
        /// <param name="type">The file output type.</param>
        /// <returns>The code</returns>
        public override string GenerateFile(ClientGeneratorOutputType type)
        {
            return GenerateFile(_service, Resolver, type);
        }

        /// <summary>Resolves the type of the parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>The parameter type name.</returns>
        protected override string ResolveParameterType(SwaggerParameter parameter, ITypeResolver resolver)
        {
            var schema = parameter.ActualSchema;
            if (schema.Type == JsonObjectType.File)
            {
                if (parameter.CollectionFormat == SwaggerParameterCollectionFormat.Multi && !schema.Type.HasFlag(JsonObjectType.Array))
                    return "IEnumerable<System.IO.Stream>";

                return "System.IO.Stream";
            }

            return base.ResolveParameterType(parameter, resolver);
        }

        internal override string RenderClientCode(string controllerName, IList<OperationModel> operations, ClientGeneratorOutputType outputType)
        {
            var template = new ClientTemplate();
            template.Initialize(new ClientTemplateModel(controllerName, operations, _service, Settings)
            {
                GenerateContracts = outputType == ClientGeneratorOutputType.Full || outputType == ClientGeneratorOutputType.Contracts,
                GenerateImplementation = outputType == ClientGeneratorOutputType.Full || outputType == ClientGeneratorOutputType.Implementation,
            });
            return template.Render();
        }
    }
}
