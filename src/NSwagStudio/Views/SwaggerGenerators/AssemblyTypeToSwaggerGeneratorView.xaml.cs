using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using MyToolkit.Mvvm;
using NSwag.CodeGeneration.Commands;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class AssemblyTypeToSwaggerGeneratorView : ISwaggerGenerator
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
