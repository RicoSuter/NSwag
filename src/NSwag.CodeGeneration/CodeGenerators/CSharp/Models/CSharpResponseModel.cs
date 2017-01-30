//-----------------------------------------------------------------------
// <copyright file="CSharpResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp.Models
{
    /// <summary>The CSharp response model.</summary>
    public class CSharpResponseModel : ResponseModelBase
    {
        /// <summary>Initializes a new instance of the <see cref="CSharpResponseModel"/> class.</summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="isSuccessResponse">Specifies whether this is the success response.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator.</param>
        public CSharpResponseModel(string statusCode, SwaggerResponse response, JsonSchema4 exceptionSchema, bool isSuccessResponse, CodeGeneratorSettingsBase settings, IClientGenerator generator)
            : base(statusCode, response, exceptionSchema, isSuccessResponse, settings, generator)
        {
        }
    }
}