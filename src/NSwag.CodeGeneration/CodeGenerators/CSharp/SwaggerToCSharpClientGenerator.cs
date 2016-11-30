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
using NSwag.CodeGeneration.CodeGenerators.CSharp.Models;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToCSharpClientGenerator : SwaggerToCSharpGeneratorBase
    {
        private static readonly string[] ReservedKeywords = new[] { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
                "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float",
                "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object",
                "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
                "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
                "ushort", "using", "virtual", "void", "volatile", "while" };

        private readonly SwaggerDocument _document;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpClientGenerator(SwaggerDocument document, SwaggerToCSharpClientGeneratorSettings settings)
            : this(document, settings, SwaggerToCSharpTypeResolver.CreateWithDefinitions(settings.CSharpGeneratorSettings, document.Definitions))
        {

        }

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpClientGenerator" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpClientGenerator(SwaggerDocument document, SwaggerToCSharpClientGeneratorSettings settings, SwaggerToCSharpTypeResolver resolver)
            : base(document, settings, resolver)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Settings = settings;
            _document = document;
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToCSharpClientGeneratorSettings Settings { get; }

        internal override ClientGeneratorBaseSettings BaseSettings => Settings;

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

        /// <summary>Resolves the type of the parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The parameter type name.</returns>
        protected override string ResolveParameterType(SwaggerParameter parameter)
        {
            var schema = parameter.ActualSchema;
            if (schema.Type == JsonObjectType.File)
            {
                if (parameter.CollectionFormat == SwaggerParameterCollectionFormat.Multi && !schema.Type.HasFlag(JsonObjectType.Array))
                    return "IEnumerable<FileParameter>";

                return "FileParameter";
            }

            return base.ResolveParameterType(parameter)
                .Replace(Settings.CSharpGeneratorSettings.ArrayType + "<", "IEnumerable<")
                .Replace(Settings.CSharpGeneratorSettings.DictionaryType + "<", "IDictionary<");
        }

        internal override string GetParameterVariableName(SwaggerParameter parameter)
        {
            var name = base.GetParameterVariableName(parameter);
            return ReservedKeywords.Contains(name) ? "@" + name : name;
        }

        internal override string GenerateClientClass(string controllerName, string controllerClassName, IList<OperationModel> operations, ClientGeneratorOutputType outputType)
        {
            var exceptionSchema = (Resolver as SwaggerToCSharpTypeResolver)?.ExceptionSchema;
            var model = new ClientTemplateModel(controllerName, controllerClassName, operations, _document, exceptionSchema, Settings)
            {
                GenerateContracts = outputType == ClientGeneratorOutputType.Full || outputType == ClientGeneratorOutputType.Contracts,
                GenerateImplementation = outputType == ClientGeneratorOutputType.Full || outputType == ClientGeneratorOutputType.Implementation,
            };

            var template = Settings.CSharpGeneratorSettings.TemplateFactory.CreateTemplate("CSharp", "Client", model);
            return template.Render();
        }
    }
}
