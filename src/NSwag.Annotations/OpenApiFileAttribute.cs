//-----------------------------------------------------------------------
// <copyright file="SwaggerFileAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies a parameter or class to be handled as file.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
#pragma warning disable 618
    public class OpenApiFileAttribute : SwaggerFileAttribute
#pragma warning restore 618
    {
    }

    /// <summary>Specifies a parameter or class to be handled as file.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    [Obsolete("Use " + nameof(OpenApiFileAttribute) + " instead.")]
    public class SwaggerFileAttribute : Attribute
    {
    }
}
