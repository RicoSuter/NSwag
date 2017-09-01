//-----------------------------------------------------------------------
// <copyright file="OperationResponseProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.SwaggerGeneration.WebApi.Processors.Models
{
    internal class OperationResponseModel
    {
        public OperationResponseModel(string httpStatusCode, Type responseType, bool isNullable, string description)
        {
            HttpStatusCode = httpStatusCode;
            ResponseType = responseType;
            IsNullable = isNullable;
            Description = description;
        }

        public string HttpStatusCode { get; }

        public Type ResponseType { get; }

        public bool IsNullable { get; }

        public string Description { get; }
    }
}