
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using NSwag.SwaggerGeneration.AspNetCore;

namespace NSwag.Commands.AspNetCore
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            //var consoleHost = new ConsoleHost();
            //var processor = new CommandLineProcessor(consoleHost);

            //processor.RegisterCommand<AspNetToSwaggerInnerCommand>();
            //processor.Process(args);

            //return 0;


            var serviceProvider = ApplicationServiceProvider.GetServiceProvider(args[0]);

            var apiDescriptionProvider = serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            var swaggerGenerator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var swaggerDocument = swaggerGenerator.GenerateAsync(apiDescriptionProvider.ApiDescriptionGroups).GetAwaiter().GetResult();
            File.WriteAllText(args[1], swaggerDocument.ToJson());

            return 0;
        }
    }
}
