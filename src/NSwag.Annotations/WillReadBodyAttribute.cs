//-----------------------------------------------------------------------
// <copyright file="WillReadBodyAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies whether the parameter or class reads the body.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class WillReadBodyAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="WillReadBodyAttribute"/> class.</summary>
        /// <param name="willReadBody">Specifies whether the parameter or class reads the body.</param>
        public WillReadBodyAttribute(bool willReadBody)
        {
            WillReadBody = willReadBody;
        }

        /// <summary>Gets or sets a value indicating whether the parameter or class reads the body.</summary>
        public bool WillReadBody { get; }
    }
}