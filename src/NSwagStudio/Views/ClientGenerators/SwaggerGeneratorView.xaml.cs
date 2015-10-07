using System;
using System.Threading.Tasks;
using System.Windows;
using NSwag;
using NSwagStudio.ViewModels.ClientGenerators;

namespace NSwagStudio.Views.ClientGenerators
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
