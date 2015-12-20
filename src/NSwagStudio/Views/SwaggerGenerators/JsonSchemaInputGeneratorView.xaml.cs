using System;
using System.Threading.Tasks;
using System.Windows;
using NJsonSchema;
using NSwag;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class JsonSchemaInputGeneratorView : ISwaggerGenerator
    {
        public JsonSchemaInputGeneratorView(NSwagDocument document)
        {
            InitializeComponent();
            DataContext = document;
        }

        public string Title { get { return "JSON Schema"; } }

        public async Task<string> GenerateSwaggerAsync()
        {
            try
            {
                var schema = JsonSchema4.FromJson(JsonSchema.Text);
                var service = new SwaggerService();
                service.Definitions[schema.TypeName ?? "MyType"] = schema;
                return service.ToJson();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error loading the JSON Schema");
                return string.Empty; // TODO: What to do on error?
            }
        }
    }
}
