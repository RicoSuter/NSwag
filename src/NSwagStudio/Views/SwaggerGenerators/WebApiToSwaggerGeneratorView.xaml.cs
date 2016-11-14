using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using MyToolkit.Mvvm;
using NSwag.CodeGeneration.Commands;
using NSwag.Commands.Base;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class WebApiToSwaggerGeneratorView : ISwaggerGeneratorView
    {
        public WebApiToSwaggerGeneratorView(WebApiToSwaggerCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command;

            ControllersList.SelectedItems.Clear();
            foreach (var controller in Model.ControllerNames)
                ControllersList.SelectedItems.Add(controller);
        }

        private WebApiToSwaggerGeneratorViewModel Model => (WebApiToSwaggerGeneratorViewModel)Resources["ViewModel"];

        public string Title => "Web API Assembly";

        public OutputCommandBase Command => Model.Command;

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
