using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation.AspNetCore.Tests.Web;

namespace NSwag.Generation.AspNetCore.Tests
{
    public class AspNetCoreTestsBase : IDisposable
    {
        public AspNetCoreTestsBase()
        {
            TestServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        protected TestServer TestServer { get; }

        protected async Task<OpenApiDocument> GenerateDocumentAsync(AspNetCoreOpenApiDocumentGeneratorSettings settings, params Type[] controllerTypes)
        {
            var generator = new AspNetCoreOpenApiDocumentGenerator(settings);
            var provider = TestServer.Host.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            var controllerTypeNames = controllerTypes.Select(t => t.FullName);
            var groups = new ApiDescriptionGroupCollection(provider.ApiDescriptionGroups.Items
                    .Select(i => new ApiDescriptionGroup(i.GroupName, i.Items
                        .Where(u => controllerTypeNames.Contains(((ControllerActionDescriptor)u.ActionDescriptor).ControllerTypeInfo.FullName))
                        .ToList()))
                    .ToList(),
                provider.ApiDescriptionGroups.Version);

            var document = await generator.GenerateAsync(groups);
            return document;
        }

        public void Dispose()
        {
            TestServer.Dispose();
        }
    }
}