using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using MyToolkit.Command;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;
using NSwag.Commands;

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
                json = await DynamicApis.HttpGetAsync(url);
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