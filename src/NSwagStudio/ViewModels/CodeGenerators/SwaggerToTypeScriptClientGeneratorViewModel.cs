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
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag;
using NSwag.CodeGeneration.CodeGenerators;
using NSwag.CodeGeneration.CodeGenerators.TypeScript;
using NSwag.Commands;

namespace NSwagStudio.ViewModels.CodeGenerators
{
    public class SwaggerToTypeScriptClientGeneratorViewModel : ViewModelBase
    {
        private string _clientCode;
        private SwaggerToTypeScriptClientCommand _command = new SwaggerToTypeScriptClientCommand();

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

        /// <summary>Gets the supported TypeScript versions.</summary>
        public decimal[] TypeScriptVersions => new[] {1.8m, 2.0m};

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

        /// <summary>Gets the list of date time types. </summary>
        public TypeScriptDateTimeType[] DateTimeTypes
        {
            get
            {
                return Enum.GetNames(typeof(TypeScriptDateTimeType))
                    .Select(t => (TypeScriptDateTimeType)Enum.Parse(typeof(TypeScriptDateTimeType), t))
                    .ToArray();
            }
        }

        public string ClassTypes
        {
            get { return _command.ClassTypes != null ? string.Join(",", _command.ClassTypes) : ""; }
            set
            {
                _command.ClassTypes = !string.IsNullOrEmpty(value) ? value.Split(',') : new string[] { };
                RaisePropertyChanged();
            }
        }

        public string ExtendedClasses
        {
            get { return _command.ExtendedClasses != null ? string.Join(",", _command.ExtendedClasses) : ""; }
            set
            {
                _command.ExtendedClasses = !string.IsNullOrEmpty(value) ? value.Split(',') : new string[] { };
                RaisePropertyChanged();
            }
        }

        /// <summary>Gets or sets the client code. </summary>
        public string ClientCode
        {
            get { return _clientCode; }
            set { Set(ref _clientCode, value); }
        }

        public Task GenerateClientAsync(string swaggerData, string documentPath)
        {
            return RunTaskAsync(async () =>
            {
                var code = string.Empty;
                await Task.Run(async () =>
                {
                    if (!string.IsNullOrEmpty(swaggerData))
                    {
                        Command.Input = SwaggerDocument.FromJson(swaggerData, documentPath);
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
