using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using MyToolkit.Command;
using Newtonsoft.Json;
using NSwag.CodeGeneration;

namespace NSwagStudio.ViewModels.SwaggerGenerators
{
    public class SwaggerInputViewModel : ViewModelBase
    {
        public SwaggerInputViewModel()
        {
            LoadSwaggerUrlCommand = new AsyncRelayCommand(async () => await LoadSwaggerUrlAsync());
        }

        public NSwagDocument Document { get; set; }

        public ICommand LoadSwaggerUrlCommand { get; }

        public async Task LoadSwaggerUrlAsync()
        {
            var json = string.Empty;
            var url = Document.InputSwaggerUrl;
            await RunTaskAsync(() =>
            {
                using (var client = new WebClient())
                    json = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(client.DownloadString(url)), Formatting.Indented);
            });
            Document.InputSwagger = json;
            Document.RaiseAllPropertiesChanged();
        }
    }
}