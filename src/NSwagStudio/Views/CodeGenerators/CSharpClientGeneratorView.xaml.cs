using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class CSharpClientGeneratorView : IClientGenerator
    {
        public CSharpClientGeneratorView(SwaggerToCSharpCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        public string Title { get { return "CSharp Client"; } }

        private CSharpClientGeneratorViewModel Model { get { return (CSharpClientGeneratorViewModel) Resources["ViewModel"]; } }
        
        public Task GenerateClientAsync(string swaggerData)
        {
            return Model.GenerateClientAsync(swaggerData);
        }
    }
}
