//-----------------------------------------------------------------------
// <copyright file="FileTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.CSharp.Templates;
using NJsonSchema;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp.Models
{
    /// <summary>The CSharp file template model.</summary>
    public class FileTemplateModel
    {
        private readonly string _clientCode;
        private readonly SwaggerDocument _document;
        private readonly SwaggerToCSharpGeneratorSettings _settings;
        private readonly SwaggerToCSharpTypeResolver _resolver;
        private readonly ClientGeneratorOutputType _outputType;
        private readonly ClientGeneratorBase _clientGeneratorBase;

        /// <summary>Initializes a new instance of the <see cref="FileTemplateModel" /> class.</summary>
        /// <param name="clientCode">The client code.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="document">The Swagger document.</param>
        /// <param name="clientGeneratorBase">The client generator base.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        public FileTemplateModel(string clientCode, ClientGeneratorOutputType outputType, SwaggerDocument document,
            ClientGeneratorBase clientGeneratorBase, SwaggerToCSharpGeneratorSettings settings, SwaggerToCSharpTypeResolver resolver)
        {
            _clientCode = clientCode;
            _outputType = outputType;
            _document = document;
            _clientGeneratorBase = clientGeneratorBase;
            _settings = settings;
            _resolver = resolver;
        }

        /// <summary>Gets the namespace.</summary>
        public string Namespace => _settings.CSharpGeneratorSettings.Namespace ?? string.Empty;

        /// <summary>Gets the all the namespace usages.</summary>
        public string[] NamespaceUsages => _outputType == ClientGeneratorOutputType.Contracts || _settings.AdditionalNamespaceUsages == null ?
            new string[] { } : _settings.AdditionalNamespaceUsages;

        /// <summary>Gets a value indicating whether to generate contract code.</summary>
        public bool GenerateContracts =>
            _outputType == ClientGeneratorOutputType.Full ||
            _outputType == ClientGeneratorOutputType.Contracts;

        /// <summary>Gets a value indicating whether to generate implementation code.</summary>
        public bool GenerateImplementation =>
            _outputType == ClientGeneratorOutputType.Full ||
            _outputType == ClientGeneratorOutputType.Implementation;

        /// <summary>Gets or sets a value indicating whether to generate client types.</summary>
        public bool GenerateClientClasses => _settings.GenerateClientClasses;

        /// <summary>Gets the clients code.</summary>
        public string Clients => _settings.GenerateClientClasses ? _clientCode : string.Empty;

        /// <summary>Gets the classes code.</summary>
        public string Classes => _settings.GenerateDtoTypes ? _resolver.GenerateClasses() : string.Empty;

        /// <summary>Gets a value indicating whether the generated code requires a JSON exception converter.</summary>
        public bool RequiresJsonExceptionConverter => JsonExceptionTypes.Any();

        /// <summary>Gets the JsonExceptionConverter code.</summary>
        public string JsonExceptionConverterCode => RequiresJsonExceptionConverter ?
            ConversionUtilities.Tab(new JsonExceptionConverterTemplate(JsonExceptionTypes.FirstOrDefault(t => t != "Exception") ?? "Exception").TransformText(), 1) : string.Empty;

        private IEnumerable<string> JsonExceptionTypes => ResponsesInheritingFromException.Select(r =>
            _clientGeneratorBase.GetType(r.ActualResponseSchema, r.IsNullable(_settings.CSharpGeneratorSettings.NullHandling), "Response"));

        private IEnumerable<SwaggerResponse> ResponsesInheritingFromException =>
            _document.Operations.SelectMany(o => o.Operation.AllResponses.Values.Where(r => r.InheritsExceptionSchema(_resolver.ExceptionSchema)));

        /// <summary>Gets a value indicating whether the generated code requires the FileParameter type.</summary>
        public bool RequiresFileParameterType => 
            _document.Operations.Any(o => o.Operation.Parameters.Any(p => p.Type.HasFlag(JsonObjectType.File)));

        /// <summary>Gets the exception class names.</summary>
        public IEnumerable<string> ExceptionClassNames
        {
            get
            {
                if (_settings is SwaggerToCSharpClientGeneratorSettings)
                {
                    var settings = (SwaggerToCSharpClientGeneratorSettings)_settings;
                    if (_settings.OperationNameGenerator.SupportsMultipleClients)
                    {
                        return _document.Operations
                            .GroupBy(o => _settings.OperationNameGenerator.GetClientName(_document, o.Path, o.Method, o.Operation))
                            .Select(g => settings.ExceptionClass.Replace("{controller}", g.Key))
                            .Distinct();
                    }
                    else
                        return new string[] { settings.ExceptionClass.Replace("{controller}", string.Empty) };
                }
                return new string[] { };
            }
        }
    }
}