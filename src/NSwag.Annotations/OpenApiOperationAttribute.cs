//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies the operation id.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OpenApiOperationAttribute : SwaggerOperationAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperationAttribute"/> class.</summary>
        /// <param name="operationId">The operation ID.</param>
        public OpenApiOperationAttribute(string operationId) : base(operationId)
        {
        }
    }

    /// <summary>Specifies the operation id.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    [Obsolete("Use " + nameof(OpenApiOperationAttribute) + " instead.")]
    public class SwaggerOperationAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperationAttribute"/> class.</summary>
        /// <param name="operationId">The operation ID.</param>
        public SwaggerOperationAttribute(string operationId)
        {
            OperationId = operationId;
        }

        /// <summary>Gets or sets the operation ID.</summary>
        public string OperationId { get; private set; }
    }
}