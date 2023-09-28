using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NSwag.Commands
{
    // Represents an application that uses Microsoft.Extensions.Hosting and supports
    // the various entry point flavors. The final model *does not* have an explicit CreateHost entry point and thus inverts the typical flow where the
    // execute Main and we wait for events to fire in order to access the appropriate state.
    // This is what allows top level statements to work, but getting the IServiceProvider is slightly more complex.
    internal static class ServiceProviderResolver
    {
        public static IServiceProvider GetServiceProvider(Assembly assembly)
        {
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));

            IServiceProvider serviceProvider = null;

            var entryPointType = assembly.EntryPoint?.DeclaringType;
            if (entryPointType != null)
            {
                var buildWebHostMethod = entryPointType.GetMethod("BuildWebHost");
                var args = Array.Empty<string>();

                if (buildWebHostMethod != null)
                {
                    var result = buildWebHostMethod.Invoke(null, new object[] { args });
                    serviceProvider = ((IWebHost)result).Services;
                }
                else
                {
                    var createWebHostMethod =
                        entryPointType?.GetRuntimeMethod("CreateWebHostBuilder", new[] { typeof(string[]) }) ??
                        entryPointType?.GetRuntimeMethod("CreateWebHostBuilder", Type.EmptyTypes);

                    if (createWebHostMethod != null)
                    {
                        var webHostBuilder = (IWebHostBuilder)createWebHostMethod.Invoke(
                            null, createWebHostMethod.GetParameters().Length > 0 ? new object[] { args } : Array.Empty<object>());
                        serviceProvider = webHostBuilder.Build().Services;
                    }
#if NETCOREAPP3_0_OR_GREATER
                    else
                    {
                        var createHostMethod =
                            entryPointType?.GetRuntimeMethod("CreateHostBuilder", new[] { typeof(string[]) }) ??
                            entryPointType?.GetRuntimeMethod("CreateHostBuilder", Type.EmptyTypes);

                        if (createHostMethod != null)
                        {
                            var webHostBuilder = (IHostBuilder)createHostMethod.Invoke(
                                null, createHostMethod.GetParameters().Length > 0 ? new object[] { args } : Array.Empty<object>());
                            serviceProvider = webHostBuilder.Build().Services;
                        }
                    }
#endif
                }
            }

            if (serviceProvider == null)
            {
                serviceProvider = GetServiceProviderWithHostFactoryResolver(assembly);
            }

            if (serviceProvider == null)
            {
                var startupType = assembly.ExportedTypes.FirstOrDefault(t => t.Name == "Startup");
                if (startupType != null)
                {
                    serviceProvider = WebHost.CreateDefaultBuilder().UseStartup(startupType).Build().Services;
                }
            }

            if (serviceProvider != null)
            {
                return serviceProvider;
            }

            throw new InvalidOperationException($"NSwag requires the assembly {assembly.GetName()} to have " +
                                                $"either an BuildWebHost or CreateWebHostBuilder/CreateHostBuilder method. " +
                                                $"See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/hosting?tabs=aspnetcore2x " +
                                                $"for suggestions on ways to refactor your startup type.");
        }

        internal static IServiceProvider GetServiceProviderWithHostFactoryResolver(Assembly assembly)
        {
#if NETFRAMEWORK
            return null;
#else
            // We're disabling the default server and the console host lifetime. This will disable:
            // 1. Listening on ports
            // 2. Logging to the console from the default host.
            // This is essentially what the test server does in order to get access to the application's
            // IServicerProvider *and* middleware pipeline.
            void ConfigureHostBuilder(object hostBuilder)
            {
                ((IHostBuilder)hostBuilder).ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IServer, NoopServer>();
                    services.AddSingleton<IHostLifetime, NoopHostLifetime>();

                    for (var i = services.Count - 1; i >= 0; i--)
                    {
                        // exclude all implementations of IHostedService
                        // except Microsoft.AspNetCore.Hosting.GenericWebHostService because that one will build/configure
                        // the WebApplication/Middleware pipeline in the case of the GenericWebHostBuilder.
                        if (typeof(IHostedService).IsAssignableFrom(services[i].ServiceType)
                            && services[i].ImplementationType is not { FullName: "Microsoft.AspNetCore.Hosting.GenericWebHostService" })
                        {
                            services.RemoveAt(i);
                        }
                    }
                });
            }

            var waitForStartTcs = new TaskCompletionSource<object>();

            void OnEntryPointExit(Exception exception)
            {
                // If the entry point exited, we'll try to complete the wait
                if (exception != null)
                {
                    waitForStartTcs.TrySetException(exception);
                }
                else
                {
                    waitForStartTcs.TrySetResult(null);
                }
            }

            // If all of the existing techniques fail, then try to resolve the ResolveHostFactory
            var factory = HostFactoryResolver.ResolveHostFactory(assembly,
                                                                 stopApplication: false,
                                                                 configureHostBuilder: ConfigureHostBuilder,
                                                                 entrypointCompleted: OnEntryPointExit);

            // We're unable to resolve the factory. This could mean the application wasn't referencing the right
            // version of hosting.
            if (factory == null)
            {
                return null;
            }

            try
            {
                // Get the IServiceProvider from the host
#if NET6_0_OR_GREATER
                var assemblyName = assembly.GetName()?.FullName ?? string.Empty;
                // We should set the application name to the startup assembly to avoid falling back to the entry assembly.
                var services = ((IHost)factory(new[] { $"--{HostDefaults.ApplicationKey}={assemblyName}" })).Services;
#else
                var services = ((IHost)factory(Array.Empty<string>())).Services;
#endif

                // Wait for the application to start so that we know it's fully configured. This is important because
                // we need the middleware pipeline to be configured before we access the ISwaggerProvider in
                // in the IServiceProvider
                var applicationLifetime = services.GetRequiredService<IHostApplicationLifetime>();

                using (var registration = applicationLifetime.ApplicationStarted.Register(() => waitForStartTcs.TrySetResult(null)))
                {
                    waitForStartTcs.Task.Wait();
                    return services;
                }
            }
            catch (InvalidOperationException)
            {
                // We're unable to resolve the host, swallow the exception and return null
            }

            return null;
#endif
        }

        private class NoopHostLifetime : IHostLifetime
        {
            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
            public Task WaitForStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        }

        private class NoopServer : IServer
        {
            public IFeatureCollection Features { get; } = new FeatureCollection();
            public void Dispose() { }
            public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken) => Task.CompletedTask;
            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        }
    }
}
