//-----------------------------------------------------------------------
// <copyright file="CSharpResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp response model.</summary>
    public class CSharpResponseModel : ResponseModelBase
    {
        /// <summary>Initializes a new instance of the <see cref="CSharpResponseModel"/> class.</summary>
        /// <param name="operationModel">The operation model.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="isPrimarySuccessResponse">Specifies whether this is the success response.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="generator">The client generator.</param>
        /// <param name="settings">The settings.</param>
        public CSharpResponseModel(IOperationModel operationModel, string statusCode, SwaggerResponse response, 
            bool isPrimarySuccessResponse, JsonSchema4 exceptionSchema, IClientGenerator generator, CodeGeneratorSettingsBase settings)
            : base(operationModel, statusCode, response, isPrimarySuccessResponse, exceptionSchema, settings, generator)
        {
        }
    }
}