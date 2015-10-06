using System;
using System.Threading.Tasks;
using System.Windows;
using NSwag;

namespace NSwagStudio.Views.ClientGenerators
{
    public partial class SwaggerGeneratorView : IClientGenerator
    {
        public SwaggerGeneratorView()
        {
            InitializeComponent();
        }

        public string Title { get { return "Swagger Specification"; } }

        public async Task GenerateClientAsync(string swaggerData)
        {
            try
            {
                SwaggerOutput.Text = SwaggerService.FromJson(swaggerData).ToJson();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception: " + exception.Message, "An error occured");
            }
        }
    }
}
