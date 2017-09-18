//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.Commands
{
    /// <summary>Enumeration of .NET runtimes where a document can be processed.</summary>
    public enum Runtime
    {
        /// <summary>Full .NET framework, x64.</summary>
        WinX64,

        /// <summary>Full .NET framework, x86.</summary>
        WinX86,

        /// <summary>.NET Core 1.0</summary>
        Core10,

        /// <summary>.NET Core 1.1</summary>
        Core11,

        /// <summary>.NET Core 2.0</summary>
        Core20,

        /// <summary>Execute in the same proces.</summary>
        Debug
    }
}