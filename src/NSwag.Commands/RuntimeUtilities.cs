//-----------------------------------------------------------------------
// <copyright file="RuntimeUtilities.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using Microsoft.Extensions.PlatformAbstractions;

namespace NSwag.Commands
{
    /// <summary>Provides runtime utilities.</summary>
    public class RuntimeUtilities
    {
        /// <summary>Gets the current runtime.</summary>
        public static Runtime CurrentRuntime
        {
            get
            {
#if NETFRAMEWORK
                return IntPtr.Size == 4 ? Runtime.WinX86 : Runtime.WinX64;
#else
                var framework = PlatformServices.Default.Application.RuntimeFramework;
                if (framework.Identifier == ".NETCoreApp")
                {
                    if (framework.Version.Major >= 8)
                    {
                        return Runtime.Net80;
                    }
					
                    if (framework.Version.Major >= 7)
                    {
                        return Runtime.Net70;
                    }

                    if (framework.Version.Major >= 6)
                    {
                        return Runtime.Net60;
                    }

                    return Runtime.Net60;
                }
                return IntPtr.Size == 4 ? Runtime.WinX86 : Runtime.WinX64;
#endif
            }
        }
    }
}
