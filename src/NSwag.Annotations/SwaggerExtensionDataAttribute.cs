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
    /// <summary>
    /// Indicates extension data to be added to the Swagger definition.
    /// </summary>
    /// <remarks>Requires the SwaggerExtensionDataOperationProcessor to be used in the Swagger definition generation.</remarks>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true)]
    public sealed class SwaggerExtensionDataAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerExtensionDataAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public SwaggerExtensionDataAttribute(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; }
    }
}