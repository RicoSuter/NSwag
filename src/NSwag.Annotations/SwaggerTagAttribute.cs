//-----------------------------------------------------------------------
// <copyright file="SwaggerTagAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies the tags for an operation.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerTagAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerTagAttribute"/> class.</summary>
        public SwaggerTagAttribute(string name)
        {
            Name = name; 
        }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the external documentation description.</summary>
        public string DocumentationDescription { get; set; }

        /// <summary>Gets or sets the external documentation URL.</summary>
        public string DocumentationUrl { get; set; }

        /// <summary>Gets or sets a value indicating whether the tags should be added to document's 'tags' property (only needed on operation methods, default: false).</summary>
        public bool AddToDocument { get; set; }
    }
}