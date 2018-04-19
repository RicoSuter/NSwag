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
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp file template model.</summary>
    public class CSharpFileTemplateModel
    {
        private readonly string _clientCode;
        private readonly SwaggerDocument _document;
        private readonly SwaggerToCSharpGeneratorSettings _settings;
        private readonly CSharpTypeResolver _resolver;
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
            CSharpTypeResolver resolver)
        {
            _clientCode = clientCode;
            _outputType = outputType;
            _document = document;
            _generator = generator;
            _settings = settings;
            _resolver = resolver;

            Classes = GenerateDtoTypes();
        }

        /// <summary>Gets the namespace.</summary>
        public string Namespace => _settings.CSharpGeneratorSettings.Namespace ?? string.Empty;

        /// <summary>Gets the all the namespace usages.</summary>
        public string[] NamespaceUsages => (_outputType == ClientGeneratorOutputType.Contracts ?
            _settings.AdditionalContractNamespaceUsages?.Where(n => n != null).ToArray() :
            _settings.AdditionalNamespaceUsages?.Where(n => n != null).ToArray()) ?? new string[] { };

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
        public string Classes { get; }

        /// <summary>Gets a value indicating whether the generated code requires a JSON exception converter.</summary>
        public bool RequiresJsonExceptionConverter => JsonExceptionTypes.Any();

        /// <summary>Gets the exception model class.</summary>
        public string ExceptionModelClass => JsonExceptionTypes.FirstOrDefault(t => t != "Exception") ?? "Exception";

        private IEnumerable<string> JsonExceptionTypes => ResponsesInheritingFromException.Select(r =>
            _generator.GetTypeName(r.ActualResponseSchema, r.IsNullable(_settings.CSharpGeneratorSettings.SchemaType), "Response"));

        private IEnumerable<SwaggerResponse> ResponsesInheritingFromException =>
            _document.Operations.SelectMany(o => o.Operation.ActualResponses.Values.Where(r => r.ActualResponseSchema?.InheritsSchema(_resolver.ExceptionSchema) == true));

        /// <summary>Gets a value indicating whether the generated code requires the FileParameter type.</summary>
        public bool RequiresFileParameterType =>
            _settings.CSharpGeneratorSettings.ExcludedTypeNames?.Contains("FileParameter") != true &&
            _document.Operations.Any(o => o.Operation.Parameters.Any(p => p.Type.HasFlag(JsonObjectType.File)));

        /// <summary>Gets a value indicating whether [generate file response class].</summary>
        public bool GenerateFileResponseClass =>
            _settings.CSharpGeneratorSettings.ExcludedTypeNames?.Contains("FileResponse") != true &&
            _document.Operations.Any(o => o.Operation.ActualResponses.Any(r => r.Value.ActualResponseSchema?.Type == JsonObjectType.File));

        /// <summary>Gets or sets a value indicating whether to generate exception classes (default: true).</summary>
        public bool GenerateExceptionClasses => (_settings as SwaggerToCSharpClientGeneratorSettings)?.GenerateExceptionClasses == true;

        /// <summary>Gets or sets a value indicating whether to wrap success responses to allow full response access.</summary>
        public bool WrapResponses => _settings.WrapResponses;

        /// <summary>Gets or sets a value indicating whether to generate the response class (only applied when WrapResponses == true, default: true).</summary>
        public bool GenerateResponseClasses => _settings.GenerateResponseClasses;

        /// <summary>Gets the response class names.</summary>
        public IEnumerable<string> ResponseClassNames
        {
            get
            {
                if (_settings.OperationNameGenerator.SupportsMultipleClients)
                {
                    return _document.Operations
                        .GroupBy(o => _settings.OperationNameGenerator.GetClientName(_document, o.Path, o.Method, o.Operation))
                        .Select(g => _settings.ResponseClass.Replace("{controller}", g.Key))
                        .Where(a => _settings.CSharpGeneratorSettings.ExcludedTypeNames?.Contains(a) != true)
                        .Distinct();
                }

                return new[] { _settings.ResponseClass.Replace("{controller}", string.Empty) };
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
                            .Where(a => _settings.CSharpGeneratorSettings.ExcludedTypeNames?.Contains(a) != true)
                            .Distinct();
                    }
                    else
                        return new[] { settings.ExceptionClass.Replace("{controller}", string.Empty) };
                }
                return new string[] { };
            }
        }

        private string GenerateDtoTypes()
        {
            var generator = new CSharpGenerator(_document, _settings.CSharpGeneratorSettings, _resolver);
            return _settings.GenerateDtoTypes ? generator.GenerateTypes().Concatenate() : string.Empty;
        }
    }
}