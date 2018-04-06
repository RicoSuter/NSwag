// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSwag.SwaggerGeneration.AspNetCore;
using NSwag.SwaggerGeneration.Processors;

namespace NSwag.Commands.SwaggerGeneration.AspNetCore
{
    /// <summary>In-process entry point for the aspnetcore2swagger command.</summary>
    internal class AspNetCoreToSwaggerGeneratorCommandEntryPoint
    {
        public static void Process(string commandContent, string outputFile, string applicationName)
        {
            var serviceProvider = GetServiceProvider(applicationName);
            var apiDescriptionProvider = serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            var settings = CreateSettings(commandContent);
            var swaggerGenerator = new AspNetCoreToSwaggerGenerator(settings);
            var swaggerDocument = swaggerGenerator.GenerateAsync(apiDescriptionProvider.ApiDescriptionGroups).GetAwaiter().GetResult();

            var outputPathDirectory = Path.GetDirectoryName(outputFile);
            Directory.CreateDirectory(outputPathDirectory);
            File.WriteAllText(outputFile, swaggerDocument.ToJson());
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

            IWebHost webHost = null;
            if (buildWebHostMethod != null)
            {
                var result = buildWebHostMethod.Invoke(null, new object[] { args });
                var resultType = result.GetType();
                webHost = (IWebHost)result;
            }
            else
            {
                var createWebHostMethod = entryPointType?.GetMethod("CreateWebHostBuilder");
                if (createWebHostMethod != null)
                {
                    var webHostBuilder = (IWebHostBuilder)createWebHostMethod.Invoke(null, new object[] { args });
                    webHost = webHostBuilder.Build();
                }
            }

            if (webHost != null)
            {
                return webHost.Services;
            }

            throw new InvalidOperationException($"aspnet2swaggercommand requires the entry point type {entryPointType.FullName} to have either an BuildWebHost or CreateWebHostBuilder method. See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/hosting?tabs=aspnetcore2x for suggestions on ways to refactor your startup type.");
        }

        private static AspNetCoreToSwaggerGeneratorSettings CreateSettings(string commandData)
        {
            var assemblyLoader = new AssemblyLoader.AssemblyLoader();
            var command = JsonConvert.DeserializeObject<AspNetCoreToSwaggerCommand>(commandData);

            var settings = command.Settings;
            if (command.DocumentProcessorTypes != null)
            {
                foreach (var type in command.DocumentProcessorTypes)
                {
                    var processor = (IDocumentProcessor)assemblyLoader.CreateInstance(type);
                    settings.DocumentProcessors.Add(processor);
                }
            }

            if (command.OperationProcessorTypes != null)
            {
                foreach (var type in command.OperationProcessorTypes)
                {
                    var processor = (IOperationProcessor)assemblyLoader.CreateInstance(type);
                    settings.OperationProcessors.Add(processor);
                }
            }

            return settings;
        }
    }
}