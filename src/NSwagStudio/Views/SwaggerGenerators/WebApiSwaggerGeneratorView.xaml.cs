using System.Threading.Tasks;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class WebApiSwaggerGeneratorView : ISwaggerGenerator
    {
        public WebApiSwaggerGeneratorView()
        {
            InitializeComponent();
        }

        private WebApiSwaggerGeneratorViewModel Model { get { return (WebApiSwaggerGeneratorViewModel)Resources["ViewModel"]; } }

        public string Title { get { return "Web API"; } }

        public Task<string> GenerateSwaggerAsync()
        {
            return Model.GenerateSwaggerAsync();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
