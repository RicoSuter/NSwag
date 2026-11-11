//-----------------------------------------------------------------------
// <copyright file="TypeScriptResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>The TypeScript response model.</summary>
    public class TypeScriptResponseModel : ResponseModelBase
    {
        private readonly TypeScriptClientGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptResponseModel" /> class.</summary>
        /// <param name="operationModel">The operation model.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="isPrimarySuccessResponse">if set to <c>true</c> [is success response].</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="settings">The settings.</param>
        public TypeScriptResponseModel(IOperationModel operationModel, OpenApiOperation operation, string statusCode, OpenApiResponse response, bool isPrimarySuccessResponse,
            JsonSchema exceptionSchema, IClientGenerator generator, TypeResolverBase resolver, TypeScriptClientGeneratorSettings settings)
            : base(operationModel, operation, statusCode, response, isPrimarySuccessResponse, exceptionSchema, resolver, settings.TypeScriptGeneratorSettings, generator)
        {
            _settings = settings;
        }

        /// <summary>Gets or sets the data conversion code.</summary>
        public string DataConversionCode { get; set; }

        /// <summary>Gets or sets a value indicating whether to use a DTO class.</summary>
        public bool UseDtoClass => _settings.TypeScriptGeneratorSettings.GetTypeStyle(Type) != TypeScriptTypeStyle.Interface;

        /// <summary>Gets a value indicating whether a null result value must be coerced to the configured
        /// response null value at runtime so it matches the declared result type. Only needed when the
        /// configured value is <see cref="TypeScriptNullValue.Undefined"/> — in <c>Null</c> mode the
        /// runtime already yields <c>null</c>, so no coercion (and no snapshot churn) is required. Also
        /// mirrors the condition under which <see cref="TypeScriptOperationModel.ResultType"/> adds a
        /// union: the response is nullable and the type is not "any" (which already allows null/undefined).</summary>
        public bool CoerceToResponseNullValue =>
            _settings.ResponseNullValue == TypeScriptNullValue.Undefined && IsNullable && Type != "any";

        /// <summary>Gets the TypeScript expression that turns the raw response text into the result value:
        /// the plain-text passthrough, or a JSON parse (with reference handling) using the reviver.</summary>
        public string DeserializeResponseTextExpression => IsPlainText
            ? "_responseText"
            : (_settings.TypeScriptGeneratorSettings.HandleReferences ? "jsonParse" : "JSON.parse") + "(_responseText, this.jsonParseReviver)";

        /// <summary>Gets the declared type of the local <c>result</c> variable. When a nullable result is
        /// coerced to the configured null value it is typed precisely (<c>T | null</c>/<c>T | undefined</c>);
        /// otherwise <c>any</c> — the permissive form the DTO conversion code is written against.</summary>
        public string ResultVariableType =>
            CoerceToResponseNullValue ? Type + " | " + ResultNullValueLiteral : "any";

        private string ResultNullValueLiteral =>
            _settings.ResponseNullValue == TypeScriptNullValue.Undefined ? "undefined" : "null";
    }
}
