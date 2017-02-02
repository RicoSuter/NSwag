//-----------------------------------------------------------------------
// <copyright file="CSharpFileTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NSwag.CodeGeneration.CSharp.Templates;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp file template model.</summary>
    public class CSharpFileTemplateModel
    {
        private readonly string _clientCode;
        private readonly SwaggerDocument _document;
        private readonly SwaggerToCSharpGeneratorSettings _settings;
        private readonly SwaggerToCSharpTypeResolver _resolver;
        private readonly ClientGeneratorOutputType _outputType;
        private readonly SwaggerToCSharpGeneratorBase _generator;

        /// <summary>Initializes a new instance of the <see cref="CSharpFileTemplateModel" /> class.</summary>
        /// <param name="clientCode">The client code.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        /// <param name="resolver">The resolver.</param>
        public CSharpFileTemplateModel(
            string clientCode, 
            ClientGeneratorOutputType outputType, 
            SwaggerDocument document, 
            SwaggerToCSharpGeneratorSettings settings,
            SwaggerToCSharpGeneratorBase generator, 
            SwaggerToCSharpTypeResolver resolver)
        {
            _clientCode = clientCode;
            _outputType = outputType;
            _document = document;
            _generator = generator;
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
            _generator.GetTypeName(r.ActualResponseSchema, r.IsNullable(_settings.CSharpGeneratorSettings.NullHandling), "Response"));

        private IEnumerable<SwaggerResponse> ResponsesInheritingFromException =>
            _document.Operations.SelectMany(o => o.Operation.AllResponses.Values.Where(r => r.InheritsExceptionSchema(_resolver.ExceptionSchema)));

        /// <summary>Gets a value indicating whether the generated code requires the FileParameter type.</summary>
        public bool RequiresFileParameterType => 
            _document.Operations.Any(o => o.Operation.Parameters.Any(p => p.Type.HasFlag(JsonObjectType.File)));

        /// <summary>Gets a value indicating whether [generate file response class].</summary>
        public bool GenerateFileResponseClass => _document.Operations
            .Any(o => o.Operation.Responses.Any(r => r.Value.ActualResponseSchema?.Type == JsonObjectType.File));

        /// <summary>Gets or sets a value indicating whether to generate exception classes (default: true).</summary>
        public bool GenerateExceptionClasses => (_settings as SwaggerToCSharpClientGeneratorSettings)?.GenerateExceptionClasses == true;

        /// <summary>Gets or sets a value indicating whether to wrap success responses to allow full response access.</summary>
        public bool WrapSuccessResponses => (_settings as SwaggerToCSharpClientGeneratorSettings)?.WrapSuccessResponses == true;

        /// <summary>Gets or sets a value indicating whether to generate the response class (only applied when WrapSuccessResponses == true, default: true).</summary>
        public bool GenerateResponseClasses => (_settings as SwaggerToCSharpClientGeneratorSettings)?.GenerateResponseClasses == true;

        /// <summary>Gets the response class names.</summary>
        public IEnumerable<string> ResponseClassNames
        {
            get
            {
                var settings = _settings as SwaggerToCSharpClientGeneratorSettings;
                if (settings != null)
                {
                    if (settings.OperationNameGenerator.SupportsMultipleClients)
                    {
                        return _document.Operations
                            .GroupBy(o => settings.OperationNameGenerator.GetClientName(_document, o.Path, o.Method, o.Operation))
                            .Select(g => settings.ResponseClass.Replace("{controller}", g.Key))
                            .Distinct();
                    }
                    else
                        return new[] { settings.ResponseClass.Replace("{controller}", string.Empty) };
                }
                return new string[] { };
            }
        }

        /// <summary>Gets the exception class names.</summary>
        public IEnumerable<string> ExceptionClassNames
        {
            get
            {
                var settings = _settings as SwaggerToCSharpClientGeneratorSettings;
                if (settings != null)
                {
                    if (settings.OperationNameGenerator.SupportsMultipleClients)
                    {
                        return _document.Operations
                            .GroupBy(o => settings.OperationNameGenerator.GetClientName(_document, o.Path, o.Method, o.Operation))
                            .Select(g => settings.ExceptionClass.Replace("{controller}", g.Key))
                            .Distinct();
                    }
                    else
                        return new[] { settings.ExceptionClass.Replace("{controller}", string.Empty) };
                }
                return new string[] { };
            }
        }
    }
}