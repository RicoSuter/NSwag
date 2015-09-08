//-----------------------------------------------------------------------
// <copyright file="SwaggerResultTypeAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies the result type of a web service method to correctly generate a Swagger definition.</summary>
    public class SwaggerResultTypeAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerResultTypeAttribute"/> class.</summary>
        /// <param name="resultType">The operation result type.</param>
        public SwaggerResultTypeAttribute(Type resultType)
        {
            ResultType = resultType; // TODO: Check for this attribute on WebAPI methods
        }

        /// <summary>Gets or sets the result type.</summary>
        public Type ResultType { get; set; }
    }
}
