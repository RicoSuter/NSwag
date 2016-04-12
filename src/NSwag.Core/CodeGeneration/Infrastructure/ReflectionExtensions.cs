//-----------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

namespace NSwag.CodeGeneration.Infrastructure
{
    /// <summary>Provides extension methods for reflection.</summary>
    public static class ReflectionExtensions
    {
        /// <summary>Checks whether the given type inherits from the given type name.</summary>
        /// <param name="type">The type.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>true if the type inherits from typeName.</returns>
        public static bool InheritsFrom(this Type type, string typeName)
        {
            var baseType = type.GetTypeInfo().BaseType;
            while (baseType != null)
            {
                if (baseType.Name == typeName)
                    return true;
                baseType = baseType.GetTypeInfo().BaseType;
            }
            return false;
        }
    }
}
