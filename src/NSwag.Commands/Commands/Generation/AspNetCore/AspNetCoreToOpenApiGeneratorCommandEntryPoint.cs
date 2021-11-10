//-----------------------------------------------------------------------
// <copyright file="AspNetCoreToSwaggerGeneratorCommandEntryPoint.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace NSwag.Commands.Generation.AspNetCore
{
    /// <summary>In-process entry point for the aspnetcore2swagger command.</summary>
    internal class AspNetCoreToOpenApiGeneratorCommandEntryPoint
    {
        public static void Process(string commandContent, string outputFile, string applicationName)
        {
            var command = JsonConvert.DeserializeObject<AspNetCoreToSwaggerCommand>(commandContent);

            var previousWorkingDirectory = command.ChangeWorkingDirectoryAndSetAspNetCoreEnvironment();
            var serviceProvider = GetServiceProvider(applicationName);

            var assemblyLoader = new AssemblyLoader.AssemblyLoader();
            var document = Task.Run(async () =>
                await command.GenerateDocumentAsync(assemblyLoader, serviceProvider, previousWorkingDirectory)).GetAwaiter().GetResult();

            var json = command.UseDocumentProvider ? document.ToJson() : document.ToJson(command.OutputType);

            var outputPathDirectory = Path.GetDirectoryName(outputFile);
            Directory.CreateDirectory(outputPathDirectory);
            File.WriteAllText(outputFile, json);
        }

        private static IServiceProvider GetServiceProvider(string applicationName)
        {
            var assemblyName = new AssemblyName(applicationName);
            var assembly = Assembly.Load(assemblyName);

            if (assembly.EntryPoint == null)
            {
                throw new InvalidOperationException($"Unable to locate the program entry point for {assemblyName}.");
            }

            var entryPointType = assembly.EntryPoint.DeclaringType;
            var buildWebHostMethod = entryPointType.GetMethod("BuildWebHost");
            var args = new string[0];

            IServiceProvider serviceProvider = null;
            if (buildWebHostMethod != null)
            {
                var result = buildWebHostMethod.Invoke(null, new object[] { args });
                serviceProvider = ((IWebHost)result).Services;
            }
            else
            {
                var createWebHostMethod =
                    entryPointType?.GetRuntimeMethod("CreateWebHostBuilder", new[] { typeof(string[]) }) ??
                    entryPointType?.GetRuntimeMethod("CreateWebHostBuilder", new Type[0]);

                if (createWebHostMethod != null)
                {
                    var webHostBuilder = (IWebHostBuilder)createWebHostMethod.Invoke(
                        null, createWebHostMethod.GetParameters().Length > 0 ? new object[] { args } : new object[0]);
                    serviceProvider = webHostBuilder.Build().Services;
                }
#if NET6_0 || NET5_0 || NETCOREAPP3_1 || NETCOREAPP3_0
                else
                {
                    var createHostMethod =
                        entryPointType?.GetRuntimeMethod("CreateHostBuilder", new[] { typeof(string[]) }) ??
                        entryPointType?.GetRuntimeMethod("CreateHostBuilder", new Type[0]);

                    if (createHostMethod != null)
                    {
                        var webHostBuilder = (Microsoft.Extensions.Hosting.IHostBuilder)createHostMethod.Invoke(
                            null, createHostMethod.GetParameters().Length > 0 ? new object[] { args } : new object[0]);
                        serviceProvider = webHostBuilder.Build().Services;
                    }
                }
#endif
            }

            if (serviceProvider != null)
            {
                return serviceProvider;
            }

            throw new InvalidOperationException($"NSwag requires the entry point type {entryPointType.FullName} to have " +
                                                $"either an BuildWebHost or CreateWebHostBuilder/CreateHostBuilder method. " +
                                                $"See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/hosting?tabs=aspnetcore2x " +
                                                $"for suggestions on ways to refactor your startup type.");
        }
    }
}
