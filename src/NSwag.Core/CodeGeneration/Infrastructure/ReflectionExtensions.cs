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
    public static class ReflectionExtensions
    {
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
