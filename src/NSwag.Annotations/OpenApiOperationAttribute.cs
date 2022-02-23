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
    /// <summary>Specifies the operation id, summary and description</summary>
    [AttributeUsage(AttributeTargets.Method)]
#pragma warning disable 618
    public class OpenApiOperationAttribute : SwaggerOperationAttribute
#pragma warning restore 618
    {
        /// <summary>Initializes a new instance of the <see cref="OpenApiOperationAttribute"/> class.</summary>
        /// <param name="operationId">The operation ID.</param>
        public OpenApiOperationAttribute(string operationId) : base(operationId)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="OpenApiOperationAttribute"/> class.</summary>
        /// <param name="summary">The operation summary.</param>
        /// /// <param name="description">The operation description.</param>
        public OpenApiOperationAttribute(string summary, string description) : base(null)
        {
            Summary = summary;
            Description = description;
        }

        /// <summary>Initializes a new instance of the <see cref="OpenApiOperationAttribute"/> class.</summary>
        /// /// <param name="operationId">The operation ID.</param>
        /// <param name="summary">The operation summary.</param>
        /// /// <param name="description">The operation description.</param>
        public OpenApiOperationAttribute(string operationId, string summary, string description) : base(operationId)
        {
            Summary = summary;
            Description = description;
        }

        /// <summary>Gets or sets the operation summary.</summary>
        public string Summary { get; private set; }

        /// <summary>Gets or sets the operation description.</summary>
        public string Description { get; private set; }
    }

    /// <summary>Specifies the operation ID.</summary>
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
