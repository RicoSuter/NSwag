//-----------------------------------------------------------------------
// <copyright file="WillReadBodyAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.Annotations
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class WillReadBodyAttribute : Attribute
    {
        public WillReadBodyAttribute(bool willReadBody)
        {
            WillReadBody = willReadBody;
        }

        public bool WillReadBody { get; }
    }
}