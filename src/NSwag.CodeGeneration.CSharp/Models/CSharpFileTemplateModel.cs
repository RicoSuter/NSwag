//-----------------------------------------------------------------------
// <copyright file="CSharpFileTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp file template model.</summary>
    public class CSharpFileTemplateModel
    {
        private readonly string _clientCode;
        private readonly OpenApiDocument _document;
        private readonly CSharpGeneratorBaseSettings _settings;
        private readonly CSharpTypeResolver _resolver;
        private readonly ClientGeneratorOutputType _outputType;
        private readonly CSharpGeneratorBase _generator;

        /// <summary>Initializes a new instance of the <see cref="CSharpFileTemplateModel" /> class.</summary>
        /// <param name="clientTypes">The client types.</param>
        /// <param name="dtoTypes">The DTO types.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        /// <param name="resolver">The resolver.</param>
        public CSharpFileTemplateModel(
            IEnumerable<CodeArtifact> clientTypes,
            IEnumerable<CodeArtifact> dtoTypes,
            ClientGeneratorOutputType outputType,
            OpenApiDocument document,
            CSharpGeneratorBaseSettings settings,
            CSharpGeneratorBase generator,
            CSharpTypeResolver resolver)
        {
            _outputType = outputType;
            _document = document;
            _generator = generator;
            _settings = settings;
            _resolver = resolver;
            _clientCode = clientTypes.Concatenate();

            Classes = dtoTypes.Concatenate();
        }

        /// <summary>Gets the namespace.</summary>
        public string Namespace => _settings.CSharpGeneratorSettings.Namespace ?? string.Empty;

        /// <summary>Gets the all the namespace usages.</summary>
        public string[] NamespaceUsages => (_outputType == ClientGeneratorOutputType.Contracts ?
            _settings.AdditionalContractNamespaceUsages?.Where(n => n != null).ToArray() :
            _settings.AdditionalNamespaceUsages?.Where(n => n != null).ToArray()) ?? new string[] { };

        /// <summary>Gets a value indicating whether the C#8 nullable reference types are enabled for this file.</summary>
        public bool GenerateNullableReferenceTypes => _settings.CSharpGeneratorSettings.GenerateNullableReferenceTypes;

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
        public bool RequiresJsonExceptionConverter =>
            _settings.CSharpGeneratorSettings.JsonLibrary == CSharpJsonLibrary.NewtonsoftJson &&
            JsonExceptionTypes.Any(); // TODO(system.text.json): How to serialize exceptions with STJ?

        /// <summary>Gets the exception model class.</summary>
        public string ExceptionModelClass => JsonExceptionTypes.FirstOrDefault(t => t != "Exception") ?? "Exception";

        private IEnumerable<string> JsonExceptionTypes => _document.Operations
            .SelectMany(o => o.Operation.ActualResponses.Where(r => r.Value.Schema?.InheritsSchema(_resolver.ExceptionSchema) == true).Select(r => new { o.Operation, Response = r.Value }))
            .Select(t => _generator.GetTypeName(t.Response.Schema, t.Response.IsNullable(_settings.CSharpGeneratorSettings.SchemaType), "Response"));

        /// <summary>Gets a value indicating whether the generated code requires the FileParameter type.</summary>
        public bool RequiresFileParameterType =>
            _settings.CSharpGeneratorSettings.ExcludedTypeNames?.Contains("FileParameter") != true &&
            (_document.Operations.Any(o => o.Operation.ActualParameters.Any(p => p.ActualTypeSchema.IsBinary)) ||
             _document.Operations.Any(o => o.Operation?.RequestBody?.Content?.Any(c => c.Value.Schema?.IsBinary == true ||
                                                                                       c.Value.Schema?.ActualSchema.ActualProperties.Any(p => p.Value.IsBinary ||
                                                                                                                                 p.Value.Item?.IsBinary == true ||
                                                                                                                                 p.Value.Items.Any(i => i.IsBinary)
                                                                                                                                 ) == true) == true));

        /// <summary>Gets a value indicating whether [generate file response class].</summary>
        public bool GenerateFileResponseClass =>
            _settings.CSharpGeneratorSettings.ExcludedTypeNames?.Contains("FileResponse") != true &&
            _document.Operations.Any(o => o.Operation.ActualResponses.Any(r => r.Value.IsBinary(o.Operation) == true));

        /// <summary>Gets or sets a value indicating whether to generate exception classes (default: true).</summary>
        public bool GenerateExceptionClasses => (_settings as CSharpClientGeneratorSettings)?.GenerateExceptionClasses == true;

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
                var settings = _settings as CSharpClientGeneratorSettings;
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
                    {
                        return new[] { settings.ExceptionClass.Replace("{controller}", string.Empty) };
                    }
                }
                return new string[] { };
            }
        }
    }
}