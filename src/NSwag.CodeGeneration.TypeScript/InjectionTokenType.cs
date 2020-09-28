//-----------------------------------------------------------------------
// <copyright file="InjectionTokenType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.TypeScript
{
    /// <summary>The Angular token type.</summary>
    public enum InjectionTokenType
    {
        /// <summary>Use the legacy/obsolete OpaqueToken (pre Angular 4).</summary>
        OpaqueToken,

        /// <summary>Use the new InjectionToken class (Angular 4+).</summary>
        InjectionToken
    }
}