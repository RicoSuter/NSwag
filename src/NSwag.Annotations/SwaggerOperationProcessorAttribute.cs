//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationProcessorAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Registers an operation processor for the given method or class.</summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerOperationProcessorAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperationProcessorAttribute"/> class.</summary>
        /// <param name="type">The type.</param>
        public SwaggerOperationProcessorAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>Gets or sets the type of the operation processor (must implement IOperationProcessor).</summary>
        public Type Type { get; set; }
    }
}