// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace NSwag.Commands.AspNetCore
{
    internal static class ApplicationServiceProvider
    {
        public static IServiceProvider GetServiceProvider(string applicationName)
        {
            var assemblyName = new AssemblyName(applicationName);
            var assembly = Assembly.Load(assemblyName);

            var entryPoint = assembly.EntryPoint?.DeclaringType;
            var buildWebHostMethod = entryPoint?.GetMethod("BuildWebHost");
            var args = new string[0];

            IWebHost webHost = null;
            if (buildWebHostMethod != null)
            {
                webHost = (IWebHost)buildWebHostMethod.Invoke(null, new object[] { args });
            }
            else
            {
                var createWebHostMethod = entryPoint?.GetMethod("CreateWebHostBuilder");
                if (createWebHostMethod != null)
                {
                    var webHostBuilder = (IWebHostBuilder)createWebHostMethod.Invoke(null, new object[] { args });
                    webHost = webHostBuilder.Build();
                }
            }
            
            if (webHost != null)
            {
                return webHost.Services
                    .GetRequiredService<IServiceScopeFactory>()
                    .CreateScope()
                    .ServiceProvider;
            }

            throw new InvalidOperationException();
        }
    }
}