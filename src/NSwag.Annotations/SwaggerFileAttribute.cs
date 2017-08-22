//-----------------------------------------------------------------------
// <copyright file="SwaggerFileAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies a parameter or class to be handled as file.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class SwaggerFileAttribute : Attribute
    {
    }
}