//-----------------------------------------------------------------------
// <copyright file="RuntimeUtilities.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.Commands
{
    /// <summary>Provides runtime utilities.</summary>
    public class RuntimeUtilities
    {
        /// <summary>Describes a supported .NET (Core) runtime.</summary>
        private sealed class CoreRuntimeInfo
        {
            public CoreRuntimeInfo(Runtime runtime, int majorVersion, string directoryName)
            {
                Runtime = runtime;
                MajorVersion = majorVersion;
                DirectoryName = directoryName;
            }

            public Runtime Runtime { get; }

            public int MajorVersion { get; }

            public string DirectoryName { get; }
        }

        /// <summary>The supported .NET (Core) runtimes, ordered by descending major version.</summary>
        private static readonly CoreRuntimeInfo[] CoreRuntimes =
        [
            new CoreRuntimeInfo(Runtime.Net110, 11, "Net110"),
            new CoreRuntimeInfo(Runtime.Net100, 10, "Net100"),
            new CoreRuntimeInfo(Runtime.Net90, 9, "Net90"),
            new CoreRuntimeInfo(Runtime.Net80, 8, "Net80"),
        ];

        /// <summary>Gets the binary directory name for the given runtime, or <c>null</c> if the runtime has none.</summary>
        /// <param name="runtime">The runtime.</param>
        /// <returns>The directory name or <c>null</c>.</returns>
        public static string GetDirectoryName(Runtime runtime)
        {
            foreach (var coreRuntime in CoreRuntimes)
            {
                if (coreRuntime.Runtime == runtime)
                {
                    return coreRuntime.DirectoryName;
                }
            }

            return null;
        }

        /// <summary>Gets the current runtime.</summary>
        public static Runtime CurrentRuntime
        {
            get
            {
#if NETFRAMEWORK
                return IntPtr.Size == 4 ? Runtime.WinX86 : Runtime.WinX64;
#else
                if (!System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.Ordinal))
                {
                    foreach (var coreRuntime in CoreRuntimes)
                    {
                        if (Environment.Version.Major >= coreRuntime.MajorVersion)
                        {
                            return coreRuntime.Runtime;
                        }
                    }

                    return Runtime.Net80;
                }
                return IntPtr.Size == 4 ? Runtime.WinX86 : Runtime.WinX64;
#endif
            }
        }
    }
}
