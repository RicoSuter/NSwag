using System.Threading.Tasks;

namespace NSwagStudio.Views.ClientGenerators
{
    public partial class SwaggerGeneratorView : IClientGenerator
    {
        public SwaggerGeneratorView()
        {
            InitializeComponent();
        }

        public string Title { get { return "Swagger"; } }

        public async Task GenerateClientAsync(string swaggerData)
        {
            SwaggerOutput.Text = swaggerData;
        }
    }
}
