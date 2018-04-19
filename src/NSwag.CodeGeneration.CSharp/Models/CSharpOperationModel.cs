//-----------------------------------------------------------------------
// <copyright file="CSharpOperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp operation model.</summary>
    public class CSharpOperationModel : OperationModelBase<CSharpParameterModel, CSharpResponseModel>
    {
        private static readonly string[] ReservedKeywords =
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
            "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float",
            "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object",
            "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
            "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "while"
        };

        private readonly SwaggerToCSharpGeneratorSettings _settings;
        private readonly SwaggerOperation _operation;
        private readonly SwaggerToCSharpGeneratorBase _generator;
        private readonly CSharpTypeResolver _resolver;

        /// <summary>Initializes a new instance of the <see cref="CSharpOperationModel" /> class.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="resolver">The resolver.</param>
        public CSharpOperationModel(
            SwaggerOperation operation,
            SwaggerToCSharpGeneratorSettings settings,
            SwaggerToCSharpGeneratorBase generator,
            CSharpTypeResolver resolver)
            : base(resolver.ExceptionSchema, operation, resolver, generator, settings)
        {
            _settings = settings;
            _operation = operation;
            _generator = generator;
            _resolver = resolver;

            var parameters = _operation.ActualParameters.ToList();

            if (settings.GenerateOptionalParameters)
            {
                if (generator is SwaggerToCSharpControllerGenerator)
                    parameters = parameters.OrderBy(p => !p.IsRequired).ThenBy(p => p.Default == null).ToList();
                else
                    parameters = parameters.OrderBy(p => !p.IsRequired).ToList();
            }

            Parameters = parameters.Select(parameter =>
                new CSharpParameterModel(parameter.Name, GetParameterVariableName(parameter, _operation.Parameters),
                    ResolveParameterType(parameter), parameter, parameters,
                    _settings.CodeGeneratorSettings,
                    _generator))
                .ToList();
        }

        /// <summary>Gets the method's access modifier.</summary>
        public string MethodAccessModifier
        {
            get
            {
                var controllerName = _settings.GenerateControllerName(ControllerName);
                var settings = _settings as SwaggerToCSharpClientGeneratorSettings;
                if (settings != null && settings.ProtectedMethods?.Contains(controllerName + "." + ConversionUtilities.ConvertToUpperCamelCase(OperationName, false) + "Async") == true)
                    return "protected";

                return "public";
            }
        }

        /// <summary>Gets the actual name of the operation (language specific).</summary>
        public override string ActualOperationName => ConversionUtilities.ConvertToUpperCamelCase(OperationName, false)
            + (MethodAccessModifier == "protected" ? "Core" : string.Empty);

        /// <summary>Gets a value indicating whether this operation is rendered as interface method.</summary>
        public bool IsInterfaceMethod => MethodAccessModifier == "public";

        /// <summary>Gets a value indicating whether the operation has a result type.</summary>
        public bool HasResult => UnwrappedResultType != "void";

        /// <summary>Gets or sets the synchronous type of the result.</summary>
        public string SyncResultType
        {
            get
            {
                if (_settings != null && WrapResponse && UnwrappedResultType != "FileResponse")
                {
                    return UnwrappedResultType == "void"
                        ? _settings.ResponseClass.Replace("{controller}", ControllerName)
                        : _settings.ResponseClass.Replace("{controller}", ControllerName) + "<" + UnwrappedResultType + ">";
                }

                return UnwrappedResultType;
            }
        }

        /// <summary>Gets or sets the type of the result.</summary>
        public override string ResultType
        {
            get
            {
                return SyncResultType == "void"
                    ? "System.Threading.Tasks.Task"
                    : "System.Threading.Tasks.Task<" + SyncResultType + ">";
            }
        }

        /// <summary>Gets or sets the type of the exception.</summary>
        public override string ExceptionType
        {
            get
            {
                if (_operation.ActualResponses.Count(r => !HttpUtilities.IsSuccessStatusCode(r.Key)) != 1)
                    return "System.Exception";

                var response = _operation.ActualResponses.Single(r => !HttpUtilities.IsSuccessStatusCode(r.Key)).Value;
                var isNullable = response.IsNullable(_settings.CodeGeneratorSettings.SchemaType);
                return _generator.GetTypeName(response.ActualResponseSchema, isNullable, "Exception");
            }
        }

        /// <summary>Gets or sets the exception descriptions.</summary>
        public IEnumerable<CSharpExceptionDescriptionModel> ExceptionDescriptions
        {
            get
            {
                var settings = (SwaggerToCSharpClientGeneratorSettings)_settings;
                var controllerName = _settings.GenerateControllerName(ControllerName);
                return Responses
                    .Where(r => r.ThrowsException)
                    .SelectMany(r =>
                    {
                        if (r.ExpectedSchemas?.Any() == true)
                        {
                            return r.ExpectedSchemas
                                .Where(s => s.Schema.ActualSchema?.InheritsSchema(_resolver.ExceptionSchema) == true)
                                .Select(s =>
                                {
                                    var schema = s.Schema;
                                    var isNullable = schema.IsNullable(_settings.CSharpGeneratorSettings.SchemaType);
                                    var typeName = _generator.GetTypeName(schema.ActualSchema, isNullable, "Response");
                                    return new CSharpExceptionDescriptionModel(typeName, s.Description, controllerName, settings);
                                });
                        }
                        else if (r.InheritsExceptionSchema)
                        {
                            return new[]
                            {
                                new CSharpExceptionDescriptionModel(r.Type, r.ExceptionDescription, controllerName, settings)
                            };
                        }
                        else
                            return new CSharpExceptionDescriptionModel[] { };
                    });
            }
        }

        /// <summary>Gets the name of the parameter variable.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <returns>The parameter variable name.</returns>
        protected override string GetParameterVariableName(SwaggerParameter parameter, IEnumerable<SwaggerParameter> allParameters)
        {
            var name = base.GetParameterVariableName(parameter, allParameters);
            return ReservedKeywords.Contains(name) ? "@" + name : name;
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
                    return "System.Collections.Generic.IEnumerable<FileParameter>";

                return "FileParameter";
            }

            if (parameter.IsBinaryBodyParameter)
            {
                return "System.IO.Stream";
            }

            return base.ResolveParameterType(parameter)
                .Replace(_settings.CSharpGeneratorSettings.ArrayType + "<", _settings.ParameterArrayType + "<")
                .Replace(_settings.CSharpGeneratorSettings.DictionaryType + "<", _settings.ParameterDictionaryType + "<");
        }

        /// <summary>Creates the response model.</summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        protected override CSharpResponseModel CreateResponseModel(string statusCode, SwaggerResponse response, JsonSchema4 exceptionSchema, IClientGenerator generator, ClientGeneratorBaseSettings settings)
        {
            return new CSharpResponseModel(this, statusCode, response, response == GetSuccessResponse(), exceptionSchema, generator, settings.CodeGeneratorSettings);
        }
    }
}
