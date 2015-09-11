using System.Threading.Tasks;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class SwaggerInputGeneratorView : ISwaggerGenerator
    {
        public SwaggerInputGeneratorView()
        {
            InitializeComponent();
        }

        public string Title { get { return "Swagger Specification"; } }

        public async Task<string> GenerateSwaggerAsync()
        {
            return SwaggerCode.Text;
        }
    }
}
