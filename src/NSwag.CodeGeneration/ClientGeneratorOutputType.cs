//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration
{
    /// <summary>Specifies the output type.</summary>
    public enum ClientGeneratorOutputType
    {
        /// <summary>A single output with contracts and implementation.</summary>
        Full,

        /// <summary>The contracts output.</summary>
        Contracts,

        /// <summary>The implementation output.</summary>
        Implementation
    }
}