//-----------------------------------------------------------------------
// <copyright file="SwaggerTagAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Indicates extension data to be added to the Swagger definition.</summary>
    /// <remarks>Requires the SwaggerExtensionDataOperationProcessor to be used in the Swagger definition generation.</remarks>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true)]
    public sealed class OpenApiExtensionDataAttribute : SwaggerExtensionDataAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerExtensionDataAttribute"/> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public OpenApiExtensionDataAttribute(string key, string value) : base(key, value)
        {
        }
    }

    /// <summary>Indicates extension data to be added to the Swagger definition.</summary>
    /// <remarks>Requires the SwaggerExtensionDataOperationProcessor to be used in the Swagger definition generation.</remarks>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true)]
    [Obsolete("Use " + nameof(OpenApiExtensionDataAttribute) + " instead.")]
    public class SwaggerExtensionDataAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerExtensionDataAttribute"/> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public SwaggerExtensionDataAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>Gets the key.</summary>
        public string Key { get; }

        /// <summary>Gets the value.</summary>
        public string Value { get; }
    }
}
