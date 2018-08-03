using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests
{
    public class AspNetCoreTestsBase<TStartup> : IDisposable 
        where TStartup : class
    {
        public AspNetCoreTestsBase()
        {
            TestServer = new TestServer(new WebHostBuilder().UseStartup<TStartup>());
        }

        protected TestServer TestServer { get; }

        protected async Task<SwaggerDocument> GenerateDocumentAsync(AspNetCoreToSwaggerGeneratorSettings settings)
        {
            var generator = new AspNetCoreToSwaggerGenerator(settings);
            var provider = TestServer.Host.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            var document = await generator.GenerateAsync(provider.ApiDescriptionGroups);
            return document;
        }

        protected SwaggerOperationDescription[] GetControllerOperations(SwaggerDocument document, string controllerName)
        {
            return document.Operations.Where(o => o.Operation.Tags.Contains(controllerName)).ToArray();
        }

        public void Dispose()
        {
            TestServer.Dispose();
        }
    }
}