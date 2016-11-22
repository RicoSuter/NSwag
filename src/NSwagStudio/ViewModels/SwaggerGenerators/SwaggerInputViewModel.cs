using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using MyToolkit.Command;
using Newtonsoft.Json;
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
            await RunTaskAsync(() =>
            {
                using (var client = new WebClient())
                    json = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(client.DownloadString(url)), Formatting.Indented);
            });

            Command.Swagger = json;
        }

        public async Task<string> GenerateSwaggerAsync()
        {
            return await RunTaskAsync(async () =>
            {
                var document = await Command.RunAsync();
                return await Task.Run(() => document?.ToJson());
            });
        }
    }
}