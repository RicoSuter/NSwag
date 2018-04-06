using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NSwag.AspNetCore.Launcher
{
    internal class Program
    {
        // Used to load NSwag.Commands into a process running with the app's dependency context
        private const string EntryPointType = "NSwag.Commands.SwaggerGeneration.AspNetCore.AspNetCoreToSwaggerGeneratorCommandEntryPoint";
        private static readonly AssemblyName CommandsAssemblyName = new AssemblyName("NSwag.Commands");

        private static readonly Version NSwagVersion = typeof(Program).GetTypeInfo().Assembly.GetName().Version;

        // List of assemblies and versions referenced by NSwag.SwaggerGeneration.AspNetCore. This represents the minimum versions
        // required to successfully run the tool.
        private static readonly Dictionary<string, AssemblyLoadInfo> NSwagReferencedAssemblies = new Dictionary<string, AssemblyLoadInfo>(StringComparer.OrdinalIgnoreCase)
        {
            ["Microsoft.AspNetCore.Authorization"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.AspNetCore.Hosting.Abstractions"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.AspNetCore.Hosting.Server.Abstractions"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.AspNetCore.Http.Abstractions"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.AspNetCore.Http"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.AspNetCore.Http.Extensions"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.AspNetCore.Http.Features"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.AspNetCore.Mvc.Abstractions"] = new AssemblyLoadInfo(new Version(1, 0, 3)),
            ["Microsoft.AspNetCore.Mvc.ApiExplorer"] = new AssemblyLoadInfo(new Version(1, 0, 3)),
            ["Microsoft.AspNetCore.Mvc.Core"] = new AssemblyLoadInfo(new Version(1, 0, 3)),
            ["Microsoft.AspNetCore.Routing.Abstractions"] = new AssemblyLoadInfo(new Version(1, 0, 3)),
            ["Microsoft.AspNetCore.Routing"] = new AssemblyLoadInfo(new Version(1, 0, 3)),
            ["Microsoft.AspNetCore.WebUtilities"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.DotNet.InternalAbstractions"] = new AssemblyLoadInfo(new Version(1, 0, 0)),
            ["Microsoft.Extensions.Configuration.Abstractions"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.Extensions.DependencyInjection.Abstractions"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.Extensions.DependencyInjection"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.Extensions.DependencyModel"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.Extensions.FileProviders.Abstractions"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.Extensions.Logging.Abstractions"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.Extensions.Logging"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.Extensions.ObjectPool"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.Extensions.Options"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Microsoft.Extensions.PlatformAbstractions"] = new AssemblyLoadInfo(new Version(1, 0, -0)),
            ["Microsoft.Extensions.Primitives"] = new AssemblyLoadInfo(new Version(1, 0, 1)),
            ["Microsoft.Net.Http.Headers"] = new AssemblyLoadInfo(new Version(1, 0, 2)),
            ["Newtonsoft.Json"] = new AssemblyLoadInfo(new Version(9, 0, 0)),
            ["NConsole"] = new AssemblyLoadInfo(new Version(3, 9, 0, 0)),
            ["NJsonSchema"] = new AssemblyLoadInfo(new Version(9, 7, 7)),
            ["NSwag.AssemblyLoader"] = new AssemblyLoadInfo(NSwagVersion),
            ["NSwag.Commands"] = new AssemblyLoadInfo(NSwagVersion),
            ["NSwag.Core"] = new AssemblyLoadInfo(NSwagVersion),
            ["NSwag.SwaggerGeneration.AspNetCore"] = new AssemblyLoadInfo(NSwagVersion),
            ["NSwag.SwaggerGeneration"] = new AssemblyLoadInfo(NSwagVersion),
            ["System.Buffers"] = new AssemblyLoadInfo(new Version(4, 0, 0)),
            ["System.Diagnostics.DiagnosticSource"] = new AssemblyLoadInfo(new Version(4, 0, 0)),
            ["System.Text.Encodings.Web"] = new AssemblyLoadInfo(new Version(4, 0, 0)),
        };

        static int Main(string[] args)
        {
            // Usage: NSwag.Console.AspNetCore [settingsFile] [toolsDirectory]
            if (args.Length < 2)
            {
                return (int)ExitCode.InsufficientArguments;
            }

            var commandFile = args[0];
            var outputFile = args[1];
            var applicationName = args[2];
            var toolsDirectory = args[3];

            if (!File.Exists(commandFile))
            {
                return (int)ExitCode.SettingsFileNotFound;
            }

            var commandContent = File.ReadAllText(commandFile);

            if (!TryLoadReferencedAssemblies())
            {
                return (int)ExitCode.VersionConflict;
            }

#if NETCOREAPP1_0
            var loadContext = System.Runtime.Loader.AssemblyLoadContext.Default;
            loadContext.Resolving += (context, assemblyName) =>
            {
                if (!NSwagReferencedAssemblies.TryGetValue(assemblyName.Name, out var assemblyInfo))
                    return null;

                var assemblyLocation = Path.Combine(toolsDirectory, assemblyName.Name + ".dll");

                if (!File.Exists(assemblyLocation))
                    throw new InvalidOperationException($"Referenced assembly '{assemblyName}' was not found in {toolsDirectory}.");

                return context.LoadFromAssemblyPath(assemblyLocation);
            };
            var assembly = loadContext.LoadFromAssemblyName(CommandsAssemblyName);
#else
            AppDomain.CurrentDomain.AssemblyResolve += (source, eventArgs) =>
            {
                var assemblyName = new AssemblyName(eventArgs.Name);
                var name = assemblyName.Name;
                if (!NSwagReferencedAssemblies.TryGetValue(name, out var assemblyInfo))
                    return null;

                // If we've loaded a higher version from the app's closure, return it.
                if (assemblyInfo.LoadedAssembly != null)
                    return assemblyInfo.LoadedAssembly;

                var assemblyLocation = Path.Combine(toolsDirectory, name + ".dll");
                if (!File.Exists(assemblyLocation))
                    throw new InvalidOperationException($"Referenced assembly '{assemblyName}' was not found in {toolsDirectory}.");
                return Assembly.LoadFile(assemblyLocation);
            };

            var assembly = Assembly.Load(CommandsAssemblyName);
#endif

            var type = assembly.GetType(EntryPointType, throwOnError: true);
            var method = type.GetMethod("Process", BindingFlags.Public | BindingFlags.Static);

            try
            {
                method.Invoke(null, new[] { commandContent, outputFile, applicationName });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return (int)ExitCode.Fail;
            }

            return 0;
        }

        private static bool TryLoadReferencedAssemblies()
        {
            foreach (var item in NSwagReferencedAssemblies)
            {

                Assembly loadedAssembly;
                try
                {
                    loadedAssembly = Assembly.Load(new AssemblyName(item.Key));
                }
                catch
                {
                    // Ignore load errors
                    continue;
                }

                var assemblyInfo = item.Value;
                if (loadedAssembly.GetName().Version < assemblyInfo.MinimumRequiredVersion)
                {
                    Console.Error.WriteLine("Application references version lower than required.");
                    return false;
                }

                assemblyInfo.LoadedAssembly = loadedAssembly;
            }

            return true;
        }

        private class AssemblyLoadInfo
        {
            public AssemblyLoadInfo(Version minimumRequiredVersion)
            {
                MinimumRequiredVersion = minimumRequiredVersion;
            }

            public Version MinimumRequiredVersion { get; }

            public Assembly LoadedAssembly { get; set; }
        }

        private enum ExitCode
        {
            Success = 0,
            Fail,
            InsufficientArguments,
            SettingsFileNotFound,
            VersionConflict,
        };
    }
}
