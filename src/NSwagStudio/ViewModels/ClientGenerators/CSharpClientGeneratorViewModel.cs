//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NSwag;
using NSwag.CodeGeneration.ClientGenerators;
using NSwag.CodeGeneration.ClientGenerators.CSharp;

namespace NSwagStudio.ViewModels.ClientGenerators
{
    public class CSharpClientGeneratorViewModel : ViewModelBase
    {
        private string _clientCode;
        private SwaggerToCSharpGeneratorSettings _settings = MainWindowModel.Settings.SwaggerToCSharpGeneratorSettings;

        /// <summary>Gets the settings.</summary>
        public SwaggerToCSharpGeneratorSettings Settings
        {
            get { return _settings; }
            set { Set(ref _settings, value); }
        }

        /// <summary>Gets the async types. </summary>
        public OperationGenerationMode[] OperationGenerationModes
        {
            get { return Enum.GetNames(typeof(OperationGenerationMode)).Select(t => (OperationGenerationMode)Enum.Parse(typeof(OperationGenerationMode), t)).ToArray(); }
        }

        /// <summary>Gets or sets the namespace usage. </summary>
        public string AdditionalNamespaceUsage
        {
            get { return Settings.AdditionalNamespaceUsages.Any() ? Settings.AdditionalNamespaceUsages.First() : null; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Settings.AdditionalNamespaceUsages = new[] { value };
                else
                    Settings.AdditionalNamespaceUsages = new string[] { };
                RaisePropertyChanged(() => AdditionalNamespaceUsage);
            }
        }

        /// <summary>Gets or sets the client code. </summary>
        public string ClientCode
        {
            get { return _clientCode; }
            set { Set(ref _clientCode, value); }
        }

        public Task GenerateClientAsync(string swaggerData)
        {
            return RunTaskAsync(async () =>
            {
                var code = string.Empty;
                await Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(swaggerData))
                    {
                        var service = SwaggerService.FromJson(swaggerData);

                        var codeGenerator = new SwaggerToCSharpGenerator(service, Settings);
                        code = codeGenerator.GenerateFile();
                    }
                });

                ClientCode = code;
            });
        }

        public override void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}
