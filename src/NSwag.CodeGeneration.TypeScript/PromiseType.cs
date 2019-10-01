//-----------------------------------------------------------------------
// <copyright file="PromiseType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.TypeScript
{
    /// <summary>The promise type.</summary>
    public enum PromiseType
    {
        /// <summary>The standard promise implementation (polyfill may be required).</summary>
        Promise,

        /// <summary>Promise from the Q promises library.</summary>
        QPromise
    }
}