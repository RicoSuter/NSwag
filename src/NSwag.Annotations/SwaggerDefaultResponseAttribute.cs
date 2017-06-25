//-----------------------------------------------------------------------
// <copyright file="SwaggerDefaultResponseAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies that a default response (HTTP 200/204) should be generated from the return type of the operation method
    /// (not needed if no response type attributes are available).</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class SwaggerDefaultResponseAttribute : Attribute
    {
    }
}