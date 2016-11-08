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
    /// <summary>Specifies the tags for an operation or a document.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SwaggerTagsAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerTagsAttribute"/> class.</summary>
        /// <param name="tags">The tags.</param>
        public SwaggerTagsAttribute(params string[] tags)
        {
            Tags = tags;
        }
        
        /// <summary>Gets the tags.</summary>
        public string[] Tags { get; private set; }

        /// <summary>Gets or sets a value indicating whether the tags should be added to document's 'tags' property (only needed on operation methods, default: false).</summary>
        public bool AddToDocument { get; set; }
    }
}