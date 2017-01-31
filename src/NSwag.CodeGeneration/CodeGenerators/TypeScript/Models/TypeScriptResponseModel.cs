//-----------------------------------------------------------------------
// <copyright file="TypeScriptResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript.Models
{
    /// <summary>The TypeScript response model.</summary>
    public class TypeScriptResponseModel : ResponseModelBase
    {
        private readonly TypeScriptGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptResponseModel" /> class.</summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="isSuccessResponse">if set to <c>true</c> [is success response].</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="settings">The settings.</param>
        public TypeScriptResponseModel(string statusCode, SwaggerResponse response, bool isSuccessResponse, JsonSchema4 exceptionSchema, IClientGenerator generator, TypeScriptGeneratorSettings settings) 
            : base(statusCode, response, isSuccessResponse, exceptionSchema, settings, generator)
        {
            _settings = settings;
        }

        /// <summary>Gets or sets the data conversion code.</summary>
        public string DataConversionCode { get; set; }

        /// <summary>Gets or sets a value indicating whether to use a DTO class.</summary>
        public bool UseDtoClass => _settings.GetTypeStyle(Type) != TypeScriptTypeStyle.Interface;
    }
}