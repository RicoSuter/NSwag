//-----------------------------------------------------------------------
// <copyright file="TypeScriptAsyncType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.ClientGenerators.TypeScript
{
    /// <summary>The asynchronism handling types.</summary>
    public enum TypeScriptAsyncType
    {
        /// <summary>Uses callbacks.</summary>
        Callbacks,

        /// <summary>Uses the Q promises library.</summary>
        Q
    }
}