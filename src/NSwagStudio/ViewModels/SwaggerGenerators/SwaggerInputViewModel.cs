using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using MyToolkit.Command;
using Newtonsoft.Json;
using NSwag.Commands;
using NJsonSchema.Infrastructure;

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
            var json = await DynamicApis.HttpGetAsync(url).ConfigureAwait(false);
            Command.Swagger = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
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