using System;
using System.Threading.Tasks;
using System.Windows;
using NJsonSchema;
using NSwag;
using NSwag.CodeGeneration;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class JsonSchemaInputView : ISwaggerGenerator
    {
        public JsonSchemaInputView(NSwagDocument document)
        {
            InitializeComponent();
            DataContext = document;
        }

        public string Title => "JSON Schema";

        public async Task<string> GenerateSwaggerAsync()
        {
            try
            {
                var schema = JsonSchema4.FromJson(JsonSchema.Text);
                var document = new SwaggerDocument();
                document.Definitions[schema.TypeNameRaw ?? "MyType"] = schema;
                return document.ToJson();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error loading the JSON Schema");
                return string.Empty; // TODO: What to do on error?
            }
        }
    }
}
