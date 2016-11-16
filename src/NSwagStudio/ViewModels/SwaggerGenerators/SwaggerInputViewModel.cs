using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using MyToolkit.Command;
using Newtonsoft.Json;
using NSwag.CodeGeneration;
using NSwag.Commands;

namespace NSwagStudio.ViewModels.SwaggerGenerators
{
    public class SwaggerInputViewModel : ViewModelBase
    {
        public SwaggerInputViewModel()
        {
            LoadSwaggerUrlCommand = new AsyncRelayCommand(async () => await LoadSwaggerUrlAsync());
        }

        public FromSwaggerCommand Command { get; set; }

        public ICommand LoadSwaggerUrlCommand { get; }

        public async Task LoadSwaggerUrlAsync()
        {
            var json = string.Empty;
            var url = Command.Url;
            await RunTaskAsync(() =>
            {
                using (var client = new WebClient())
                    json = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(client.DownloadString(url)), Formatting.Indented);
            });
            Command.Swagger = json;
        }
    }
}