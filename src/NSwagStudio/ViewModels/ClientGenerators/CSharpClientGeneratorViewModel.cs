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
using NSwag.Commands;

namespace NSwagStudio.ViewModels.ClientGenerators
{
    public class CSharpClientGeneratorViewModel : ViewModelBase
    {
        private string _clientCode;
        private SwaggerToCSharpCommand _command = MainWindowModel.Settings.SwaggerToCSharpCommand;

        /// <summary>Gets the settings.</summary>
        public SwaggerToCSharpCommand Command
        {
            get { return _command; }
            set { Set(ref _command, value); }
        }

        /// <summary>Gets the async types. </summary>
        public OperationGenerationMode[] OperationGenerationModes
        {
            get { return Enum.GetNames(typeof(OperationGenerationMode)).Select(t => (OperationGenerationMode)Enum.Parse(typeof(OperationGenerationMode), t)).ToArray(); }
        }

        /// <summary>Gets or sets the namespace usage. </summary>
        public string AdditionalNamespaceUsage
        {
            get
            {
                return Command.AdditionalNamespaceUsages != null && 
                    Command.AdditionalNamespaceUsages.Any() ? Command.AdditionalNamespaceUsages.First() : null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Command.AdditionalNamespaceUsages = new[] { value };
                else
                    Command.AdditionalNamespaceUsages = new string[] { };
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
                await Task.Run(async () =>
                {
                    if (!string.IsNullOrEmpty(swaggerData))
                    {
                        Command.Input = swaggerData;
                        code = await Command.RunAsync();
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
