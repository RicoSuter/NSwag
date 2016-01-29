using System.Threading.Tasks;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerGeneratorView : IClientGenerator
    {
        public SwaggerGeneratorView()
        {
            InitializeComponent();
        }

        public string Title { get { return "Swagger Specification"; } }

        private SwaggerGeneratorViewModel Model { get { return (SwaggerGeneratorViewModel)Resources["ViewModel"]; } }

        public Task GenerateClientAsync(string swaggerData)
        {
            return Model.GenerateClientAsync(swaggerData);
        }
    }
}
