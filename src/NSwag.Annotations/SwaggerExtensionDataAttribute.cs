//-----------------------------------------------------------------------
// <copyright file="JsonSchemaExtensionDataAttribute.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Adds an extension data property to a class or property.</summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class SwaggerExtensionDataAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerExtensionDataAttribute"/> class.</summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public SwaggerExtensionDataAttribute(string property, object value)
        {
            Property = property;
            Value = value;
        }

        /// <summary>Gets the property name.</summary>
        public string Property { get; private set; }

        /// <summary>Gets the value.</summary>
        public object Value { get; private set; }
    }
}