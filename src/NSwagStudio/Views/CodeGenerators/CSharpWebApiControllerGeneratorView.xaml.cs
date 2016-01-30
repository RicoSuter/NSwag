using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class CSharpWebApiControllerGeneratorView : ICodeGenerator
    {
        public CSharpWebApiControllerGeneratorView(SwaggerToCSharpWebApiControllerCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        public string Title { get { return "CSharp Web API Controller (beta)"; } }

        private CSharpWebApiControllerGeneratorViewModel Model { get { return (CSharpWebApiControllerGeneratorViewModel) Resources["ViewModel"]; } }
        
        public Task GenerateClientAsync(string swaggerData)
        {
            return Model.GenerateClientAsync(swaggerData);
        }
    }
}
