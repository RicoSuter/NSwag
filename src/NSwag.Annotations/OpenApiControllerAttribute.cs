//-----------------------------------------------------------------------
// <copyright file="OpenApiControllerAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Describes the controller.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OpenApiControllerAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="OpenApiOperationAttribute"/> class.</summary>
        /// <param name="name">The controller name used in OpenAPI specs (will be the generated client class name in NSwag).</param>
        public OpenApiControllerAttribute(string name)
        {
            Name = name;
        }

        /// <summary>Gets or sets the controller name used in OpenAPI specs (will be the generated client class name in NSwag).</summary>
        public string Name { get; private set; }
    }
}
