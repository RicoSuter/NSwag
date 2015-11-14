using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwagStudio.ViewModels.ClientGenerators;

namespace NSwagStudio.Views.ClientGenerators
{
    public partial class CSharpClientGeneratorView : IClientGenerator
    {
        public CSharpClientGeneratorView()
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
        }

        public string Title { get { return "CSharp Client"; } }

        private CSharpClientGeneratorViewModel Model { get { return (CSharpClientGeneratorViewModel) Resources["ViewModel"]; } }

        public Task GenerateClientAsync(string swaggerData)
        {
            return Model.GenerateClientAsync(swaggerData);
        }
    }
}
