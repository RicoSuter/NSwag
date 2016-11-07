//-----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;

namespace NSwag.CodeGeneration.Infrastructure
{
    internal static class ObjectExtensions
    {
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj?.GetType().GetRuntimeProperty(propertyName) != null;
        }
    }
}
