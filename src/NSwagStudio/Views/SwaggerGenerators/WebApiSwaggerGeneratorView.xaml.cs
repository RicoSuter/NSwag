using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MyToolkit.Mvvm;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Commands;
using NSwagStudio.ViewModels;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class WebApiSwaggerGeneratorView : ISwaggerGenerator
    {
        public WebApiSwaggerGeneratorView(WebApiToSwaggerCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command;

            ControllersList.SelectedItems.Clear();
            foreach (var controller in Model.Command.ControllerNames)
                ControllersList.SelectedItems.Add(controller);
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

        private void ControllersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.ControllerNames = ((ListBox) sender).SelectedItems.OfType<string>().ToArray();
        }
    }
}
