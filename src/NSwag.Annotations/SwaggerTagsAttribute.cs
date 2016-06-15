//-----------------------------------------------------------------------
// <copyright file="SwaggerTagsAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies the tags for an operation.</summary>
    public class SwaggerTagsAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="ResponseTypeAttribute"/> class.</summary>
        /// <param name="tags">The tags.</param>
        public SwaggerTagsAttribute(params string[] tags)
        {
            Tags = tags;
        }
        
        /// <summary>Gets the tags.</summary>
        public string[] Tags { get; private set; }
    }
}