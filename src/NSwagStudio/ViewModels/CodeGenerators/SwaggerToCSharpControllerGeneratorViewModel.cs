//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.Commands.CodeGeneration;

namespace NSwagStudio.ViewModels.CodeGenerators
{
    public class SwaggerToCSharpControllerGeneratorViewModel : ViewModelBase
    {
        private string _clientCode;
        private SwaggerToCSharpControllerCommand _command = new SwaggerToCSharpControllerCommand();

        /// <summary>Gets the settings.</summary>
        public SwaggerToCSharpControllerCommand Command
        {
            get { return _command; }
            set
            {
                if (Set(ref _command, value))
                    RaiseAllPropertiesChanged();
            }
        }

        /// <summary>Gets the list of operation modes. </summary>
        public OperationGenerationMode[] OperationGenerationModes { get; } = Enum.GetNames(typeof(OperationGenerationMode))
            .Select(t => (OperationGenerationMode)Enum.Parse(typeof(OperationGenerationMode), t))
            .ToArray();

        /// <summary>Gets the list of class styles. </summary>
        public CSharpClassStyle[] ClassStyles { get; } = Enum.GetNames(typeof(CSharpClassStyle))
            .Select(t => (CSharpClassStyle)Enum.Parse(typeof(CSharpClassStyle), t))
            .ToArray();

        /// <summary>Gets the list of class styles. </summary>
        public CSharpControllerStyle[] ControllerStyles { get; } = Enum.GetNames(typeof(CSharpControllerStyle))
            .Select(t => (CSharpControllerStyle)Enum.Parse(typeof(CSharpControllerStyle), t))
            .ToArray();

        /// <summary>Gets or sets the client code. </summary>
        public string ClientCode
        {
            get { return _clientCode; }
            set { Set(ref _clientCode, value); }
        }
    }
}
