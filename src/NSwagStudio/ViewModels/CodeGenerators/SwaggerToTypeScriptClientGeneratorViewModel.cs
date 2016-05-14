//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptClientGeneratorModel.cs" company="NSwag">
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
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.CodeGenerators;
using NSwag.CodeGeneration.CodeGenerators.TypeScript;
using NSwag.Commands;

namespace NSwagStudio.ViewModels.CodeGenerators
{
    public class SwaggerToTypeScriptClientGeneratorViewModel : ViewModelBase
    {
        private string _clientCode;
        private SwaggerToTypeScriptClientCommand _command = new SwaggerToTypeScriptClientCommand();

        public bool ShowSettings
        {
            get { return ApplicationSettings.GetSetting("SwaggerToTypeScriptClientGeneratorModel.ShowSettings", true); }
            set { ApplicationSettings.SetSetting("SwaggerToTypeScriptClientGeneratorModel.ShowSettings", value); }
        }

        /// <summary>Gets the settings.</summary>
        public SwaggerToTypeScriptClientCommand Command
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

        /// <summary>Gets the operation modes. </summary>
        public OperationGenerationMode[] OperationGenerationModes
        {
            get { return Enum.GetNames(typeof(OperationGenerationMode)).Select(t => (OperationGenerationMode)Enum.Parse(typeof(OperationGenerationMode), t)).ToArray(); }
        }

        /// <summary>Gets the promise types. </summary>
        public PromiseType[] PromiseTypes
        {
            get { return Enum.GetNames(typeof(PromiseType)).Select(t => (PromiseType)Enum.Parse(typeof(PromiseType), t)).ToArray(); }
        }

        /// <summary>Gets the list of type styles. </summary>
        public TypeScriptTypeStyle[] TypeStyles
        {
            get
            {
                return Enum.GetNames(typeof(TypeScriptTypeStyle))
                    .Select(t => (TypeScriptTypeStyle)Enum.Parse(typeof(TypeScriptTypeStyle), t))
                    .ToArray();
            }
        }

        public string ClassMappings
        {
            get { return _command.ClassMappings != null ? JsonConvert.SerializeObject(_command.ClassMappings, Formatting.Indented) : "[]"; }
            set
            {
                try
                {
                    _command.ClassMappings = JsonConvert.DeserializeObject<TypeScriptClassMapping[]>(value);
                }
                catch
                {
                    _command.ClassMappings = new TypeScriptClassMapping[] { };
                }
                RaisePropertyChanged();
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
                        Command.Input = null;
                    }
                });
                ClientCode = code ?? string.Empty;
            });
        }

        public override void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}
