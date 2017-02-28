//-----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;

namespace NSwag.SwaggerGeneration.WebApi.Infrastructure
{
    /// <summary>Object reflection extensions.</summary>
    public static class ObjectExtensions
    {
        /// <summary>Determines whether the specified property name exists.</summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if the property exists; otherwise, <c>false</c>.</returns>
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj?.GetType().GetRuntimeProperty(propertyName) != null;
        }

        /// <summary>Determines whether the specified property name exists.</summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="defaultValue">Default value if the property does not exist.</param>
        /// <returns><c>true</c> if the property exists; otherwise, <c>false</c>.</returns>
        public static T TryGetPropertyValue<T>(this object obj, string propertyName, T defaultValue)
        {
            var property = obj?.GetType().GetRuntimeProperty(propertyName);

            return property == null ? defaultValue : (T)property.GetValue(obj);
        }
    }
}
