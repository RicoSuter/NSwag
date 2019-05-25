//-----------------------------------------------------------------------
// <copyright file="OperationResponseDescription.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Generation.Processors
{
    /// <summary>Describes an operation response.</summary>
    public class OperationResponseDescription
    {
        /// <summary>Initializes a new instance of the <see cref="OperationResponseDescription"/> class with 'Bearer' name.</summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="responseType">The response type.</param>
        /// <param name="isNullable">Specifies whether the response is nullable.</param>
        /// <param name="description">The description of the response.</param>
        public OperationResponseDescription(string statusCode, Type responseType, bool isNullable, string description)
        {
            StatusCode = statusCode;
            ResponseType = responseType;
            IsNullable = isNullable;
            Description = description;
        }

        /// <summary>Gets the HTTP status code.</summary>
        public string StatusCode { get; }

        /// <summary>Gets the response type..</summary>
        public Type ResponseType { get; }

        /// <summary>Gets a value indicating whether the response is nullable.</summary>
        public bool IsNullable { get; }

        /// <summary>Gets description.</summary>
        public string Description { get; }
    }
}