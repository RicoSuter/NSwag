using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwag.Commands.SwaggerGeneration.WebApi;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class WebApiToSwaggerGeneratorView : ISwaggerGeneratorView
    {
        public WebApiToSwaggerGeneratorView(WebApiToSwaggerCommand command, NSwagDocument document)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);

            Model.Command = command;
            Model.Document = document;

            ControllersList.SelectedItems.Clear();
            foreach (var controller in Model.ControllerNames)
                ControllersList.SelectedItems.Add(controller);
        }

        private WebApiToSwaggerGeneratorViewModel Model => (WebApiToSwaggerGeneratorViewModel)Resources["ViewModel"];

        public string Title => "Web API or ASP.NET Core Assembly";

        public IOutputCommand Command => Model.Command;

        public Task<string> GenerateSwaggerAsync()
        {
            return Model.GenerateSwaggerAsync();
        }

        public override string ToString()
        {
            return Title;
        }

        private void ControllersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.ControllerNames = ((ListBox) sender).SelectedItems.OfType<string>().ToArray();
        }
    }
}
