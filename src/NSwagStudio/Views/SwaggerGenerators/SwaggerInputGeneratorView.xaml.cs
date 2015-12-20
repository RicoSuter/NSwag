using System.Threading.Tasks;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class SwaggerInputGeneratorView : ISwaggerGenerator
    {
        public SwaggerInputGeneratorView(NSwagDocument document)
        {
            InitializeComponent();
            DataContext = document; 
        }

        public string Title { get { return "Swagger Specification"; } }

        public async Task<string> GenerateSwaggerAsync()
        {
            return SwaggerCode.Text;
        }
    }
}
