//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Command;
using MyToolkit.Mvvm;
using NSwagStudio.Views.ClientGenerators;
using NSwagStudio.Views.SwaggerGenerators;

namespace NSwagStudio.ViewModels
{
    public class MainWindowModel : ViewModelBase
    {
        private ISwaggerGenerator _selectedSwaggerGenerator;

        private readonly ISwaggerGenerator[] _swaggerGenerators = new ISwaggerGenerator[]
        {
            new SwaggerInputGeneratorView(),
            new WebApiSwaggerGeneratorView(),
            new AssemblySwaggerGeneratorView(), 
        };

        private readonly IClientGenerator[] _clientGenerators = new IClientGenerator[]
        {
            new SwaggerGeneratorView(),
            new TypeScriptCodeGeneratorView(),
            new CSharpClientGeneratorView()
        };

        public MainWindowModel()
        {
            GenerateCommand = new AsyncRelayCommand(GenerateAsync);
            SelectedSwaggerGenerator = SwaggerGenerators.First();
        }

        public AsyncRelayCommand GenerateCommand { get; set; }

        public ISwaggerGenerator[] SwaggerGenerators { get { return _swaggerGenerators; } }

        public IClientGenerator[] ClientGenerators { get { return _clientGenerators; } }

        /// <summary>Gets or sets the selected <see cref="ISwaggerGenerator"/>. </summary>
        public ISwaggerGenerator SelectedSwaggerGenerator
        {
            get { return _selectedSwaggerGenerator; }
            set { Set(ref _selectedSwaggerGenerator, value); }
        }

        private async Task GenerateAsync()
        {
            if (SelectedSwaggerGenerator != null)
            {
                var swaggerCode = await SelectedSwaggerGenerator.GenerateSwaggerAsync();
                foreach (var generator in ClientGenerators)
                    await generator.GenerateClientAsync(swaggerCode);
            }
        }
    }
}
