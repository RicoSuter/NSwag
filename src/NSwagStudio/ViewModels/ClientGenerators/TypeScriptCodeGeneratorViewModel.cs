//-----------------------------------------------------------------------
// <copyright file="TypeScriptCodeGeneratorViewModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyToolkit.Storage;
using NSwag;
using NSwag.CodeGeneration.ClientGenerators;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;
using NSwag.Commands;

namespace NSwagStudio.ViewModels.ClientGenerators
{
    public class TypeScriptCodeGeneratorViewModel : ViewModelBase
    {
        private string _clientCode;
        private SwaggerToTypeScriptCommand _command = new SwaggerToTypeScriptCommand();

        public bool ShowSettings
        {
            get { return ApplicationSettings.GetSetting("TypeScriptCodeGeneratorViewModel.ShowSettings", true); }
            set { ApplicationSettings.SetSetting("TypeScriptCodeGeneratorViewModel.ShowSettings", value); }
        }

        /// <summary>Gets the settings.</summary>
        public SwaggerToTypeScriptCommand Command
        {
            get { return _command; }
            set
            {
                if (Set(ref _command, value))
                    RaiseAllPropertiesChanged();
            }
        }

        /// <summary>Gets the output templates. </summary>
        public TypeScriptTemplate[] Templates
        {
            get { return Enum.GetNames(typeof(TypeScriptTemplate)).Select(t => (TypeScriptTemplate)Enum.Parse(typeof(TypeScriptTemplate), t)).ToArray(); }
        }
        
        /// <summary>Gets the async types. </summary>
        public OperationGenerationMode[] OperationGenerationModes
        {
            get { return Enum.GetNames(typeof(OperationGenerationMode)).Select(t => (OperationGenerationMode)Enum.Parse(typeof(OperationGenerationMode), t)).ToArray(); }
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
                        Command.Input = null;
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
