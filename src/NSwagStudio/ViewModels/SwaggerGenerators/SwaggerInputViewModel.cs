using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MyToolkit.Command;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;
using NSwag.Commands.SwaggerGeneration;

namespace NSwagStudio.ViewModels.SwaggerGenerators
{
    public class SwaggerInputViewModel : ViewModelBase
    {
        public SwaggerInputViewModel()
        {
            LoadSwaggerUrlCommand = new AsyncRelayCommand<string>(async url => await LoadSwaggerUrlAsync(url));
        }

        public FromSwaggerCommand Command { get; set; }

        public ICommand LoadSwaggerUrlCommand { get; }

        public async Task LoadSwaggerUrlAsync(string url)
        {
            var json = string.Empty;
            await RunTaskAsync(async () =>
            {
                json = url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || 
                       url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) ? 
                    await DynamicApis.HttpGetAsync(url) : await DynamicApis.FileReadAllTextAsync(url);
                json = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
            });

            Command.Swagger = json;
        }

        public async Task<string> GenerateSwaggerAsync()
        {
            return await RunTaskAsync(async () =>
            {
                return await Task.Run(async () =>
                {
                    var document = await Command.RunAsync().ConfigureAwait(false);
                    return document.ToJson();
                });
            });
        }
    }
}