using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using MyToolkit.Mvvm;
using Newtonsoft.Json;
using NSwag.CodeGeneration.Commands;
using NSwag.Commands.Base;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class AssemblyTypeToSwaggerGeneratorView : ISwaggerGeneratorView
    {
        public AssemblyTypeToSwaggerGeneratorView(AssemblyTypeToSwaggerCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command;

            ControllersList.SelectedItems.Clear();
            foreach (var controller in Model.ClassNames)
                ControllersList.SelectedItems.Add(controller);
        }

        private AssemblyTypeToSwaggerGeneratorViewModel Model => (AssemblyTypeToSwaggerGeneratorViewModel)Resources["ViewModel"];

        public string Title => ".NET Assembly";

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
            Model.ClassNames = ((ListBox)sender).SelectedItems.OfType<string>().ToArray();
        }
    }
}
