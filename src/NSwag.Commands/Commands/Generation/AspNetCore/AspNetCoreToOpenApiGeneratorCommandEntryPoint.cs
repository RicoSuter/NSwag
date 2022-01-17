//-----------------------------------------------------------------------
// <copyright file="AspNetCoreToSwaggerGeneratorCommandEntryPoint.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.IO;
using System.Reflection;
using Newtonsoft.Json;

#pragma warning disable CS0618

namespace NSwag.Commands.Generation.AspNetCore
{
    /// <summary>In-process entry point for the aspnetcore2swagger command.</summary>
    internal class AspNetCoreToOpenApiGeneratorCommandEntryPoint
    {
        public static void Process(string commandContent, string outputFile, string applicationName)
        {
            var command = JsonConvert.DeserializeObject<AspNetCoreToSwaggerCommand>(commandContent);
            var previousWorkingDirectory = command.ChangeWorkingDirectoryAndSetAspNetCoreEnvironment();

            var assemblyName = new AssemblyName(applicationName);
            var assembly = Assembly.Load(assemblyName);
            var serviceProvider = ServiceProviderResolver.GetServiceProvider(assembly);

            var assemblyLoader = new AssemblyLoader.AssemblyLoader();
            var document = command.GenerateDocumentAsync(assemblyLoader, serviceProvider, previousWorkingDirectory).GetAwaiter().GetResult();

            var json = command.UseDocumentProvider ? document.ToJson() : document.ToJson(command.OutputType);

            var outputPathDirectory = Path.GetDirectoryName(outputFile);
            Directory.CreateDirectory(outputPathDirectory);
            File.WriteAllText(outputFile, json);
        }
    }
}
