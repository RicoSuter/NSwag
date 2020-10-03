//-----------------------------------------------------------------------
// <copyright file="OpenApiBodyParameterAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies that the operation consumes the POST body.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OpenApiBodyParameterAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="OpenApiBodyParameterAttribute"/> class with the 'application/json' mime type.</summary>
        public OpenApiBodyParameterAttribute()
        {
            MimeTypes = new[] { "application/json" };
        }

        /// <summary>Initializes a new instance of the <see cref="OpenApiBodyParameterAttribute"/> class.</summary>
        /// <param name="mimeTypes">The expected mime types.</param>
        public OpenApiBodyParameterAttribute(params string[] mimeTypes)
        {
            MimeTypes = mimeTypes;
        }

        /// <summary>
        /// Gets the expected body mime type.
        /// </summary>
        public string[] MimeTypes { get; }
    }
}