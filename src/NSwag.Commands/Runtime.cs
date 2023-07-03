//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.Commands
{
    /// <summary>Enumeration of .NET runtimes where a document can be processed.</summary>
    public enum Runtime
    {
        /// <summary>Use default and do no checks.</summary>
        Default,

        /// <summary>Full .NET framework, x64.</summary>
        WinX64,

        /// <summary>Full .NET framework, x86.</summary>
        WinX86,

        /// <summary>.NET Core 2.1 app.</summary>
        NetCore21,

        /// <summary>.NET Core 3.1 app.</summary>
        NetCore31,

        /// <summary>.NET 5 app.</summary>
        Net50,

        /// <summary>.NET 6 app.</summary>
        Net60,

        /// <summary>.NET 7 app.</summary>
        Net70,

        /// <summary>Execute in the same process.</summary>
        Debug
    }
}
