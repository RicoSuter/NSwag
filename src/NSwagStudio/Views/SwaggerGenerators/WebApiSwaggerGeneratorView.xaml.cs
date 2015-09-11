using System.IO;
using System.Threading.Tasks;
using System.Windows;
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

        public string Title { get { return "Web API Assembly"; } }

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
