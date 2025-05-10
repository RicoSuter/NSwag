﻿//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationProcessorAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.Annotations
{
    /// <summary>Registers an operation processor for the given method or class.</summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
#pragma warning disable 618
    public class OpenApiOperationProcessorAttribute : SwaggerOperationProcessorAttribute
#pragma warning restore 618
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperationProcessorAttribute"/> class.</summary>
        /// <param name="type">The operation processor type (must implement IOperationProcessor).</param>
        /// <param name="parameters">The parameters.</param>
        public OpenApiOperationProcessorAttribute(Type type, params object[] parameters) : base(type, parameters)
        {
        }
    }

    /// <summary>Registers an operation processor for the given method or class.</summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [Obsolete("Use " + nameof(OpenApiOperationProcessorAttribute) + " instead.")]
    public class SwaggerOperationProcessorAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperationProcessorAttribute"/> class.</summary>
        /// <param name="type">The operation processor type (must implement IOperationProcessor).</param>
        /// <param name="parameters">The parameters.</param>
        public SwaggerOperationProcessorAttribute(Type type, params object[] parameters)
        {
            Type = type;
            Parameters = parameters;
        }

        /// <summary>Gets or sets the type of the operation processor (must implement IOperationProcessor).</summary>
        public Type Type { get; set; }

        /// <summary>Gets or sets the type of the constructor parameters.</summary>
        public object[] Parameters { get; set; }
    }
}
