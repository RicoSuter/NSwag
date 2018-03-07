//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptClientGeneratorModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Commands.CodeGeneration;

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
        public decimal[] TypeScriptVersions => new[] { 1.8m, 2.0m, 2.4m, 2.7m };

        /// <summary>Gets the output templates.</summary>
        public TypeScriptTemplate[] Templates => Enum.GetNames(typeof(TypeScriptTemplate))
            .Select(t => (TypeScriptTemplate)Enum.Parse(typeof(TypeScriptTemplate), t))
            .ToArray();

        /// <summary>Gets the operation modes.</summary>
        public OperationGenerationMode[] OperationGenerationModes => Enum.GetNames(typeof(OperationGenerationMode))
            .Select(t => (OperationGenerationMode)Enum.Parse(typeof(OperationGenerationMode), t))
            .ToArray();

        /// <summary>Gets the promise types.</summary>
        public PromiseType[] PromiseTypes => Enum.GetNames(typeof(PromiseType))
            .Select(t => (PromiseType)Enum.Parse(typeof(PromiseType), t))
            .ToArray();

        /// <summary>Gets the promise types.</summary>
        public HttpClass[] HttpClasses => Enum.GetNames(typeof(HttpClass))
            .Select(t => (HttpClass)Enum.Parse(typeof(HttpClass), t))
            .ToArray();

        /// <summary>Gets the promise types.</summary>
        public InjectionTokenType[] InjectionTokenTypes => Enum.GetNames(typeof(InjectionTokenType))
            .Select(t => (InjectionTokenType)Enum.Parse(typeof(InjectionTokenType), t))
            .ToArray();

        /// <summary>Gets the list of type styles.</summary>
        public TypeScriptTypeStyle[] TypeStyles => Enum.GetNames(typeof(TypeScriptTypeStyle))
            .Select(t => (TypeScriptTypeStyle)Enum.Parse(typeof(TypeScriptTypeStyle), t))
            .ToArray();

        /// <summary>Gets the list of date time types.</summary>
        public TypeScriptDateTimeType[] DateTimeTypes => Enum.GetNames(typeof(TypeScriptDateTimeType))
            .Select(t => (TypeScriptDateTimeType)Enum.Parse(typeof(TypeScriptDateTimeType), t))
            .ToArray();

        /// <summary>Gets the list of null values.</summary>
        public TypeScriptNullValue[] NullValues => Enum.GetNames(typeof(TypeScriptNullValue))
            .Select(t => (TypeScriptNullValue)Enum.Parse(typeof(TypeScriptNullValue), t))
            .ToArray();

        /// <summary>Gets or sets the client code.</summary>
        public string ClientCode
        {
            get { return _clientCode; }
            set { Set(ref _clientCode, value); }
        }
    }
}
