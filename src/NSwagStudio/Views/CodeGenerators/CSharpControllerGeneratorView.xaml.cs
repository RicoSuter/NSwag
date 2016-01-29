using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class CSharpControllerGeneratorView : ICodeGenerator
    {
        public CSharpControllerGeneratorView(SwaggerToCSharpControllerCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        public string Title { get { return "CSharp Web API Controller (beta)"; } }

        private CSharpControllerGeneratorViewModel Model { get { return (CSharpControllerGeneratorViewModel) Resources["ViewModel"]; } }
        
        public Task GenerateClientAsync(string swaggerData)
        {
            return Model.GenerateClientAsync(swaggerData);
        }
    }
}
