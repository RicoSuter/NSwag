﻿//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;

namespace NSwagStudio.ViewModels.CodeGenerators
{
    public class SwaggerToCSharpControllerGeneratorViewModel : ViewModelBase
    {
        private string _clientCode;
        private OpenApiToCSharpControllerCommand _command = new OpenApiToCSharpControllerCommand();

        /// <summary>Gets the settings.</summary>
        public OpenApiToCSharpControllerCommand Command
        {
            get { return _command; }
            set
            {
                if (Set(ref _command, value))
                    RaiseAllPropertiesChanged();
            }
        }

        /// <summary>Gets the list of operation modes.</summary>
        public OperationGenerationMode[] OperationGenerationModes { get; } = Enum.GetNames(typeof(OperationGenerationMode))
            .Select(t => (OperationGenerationMode)Enum.Parse(typeof(OperationGenerationMode), t))
            .ToArray();

        /// <summary>Gets the list of class styles.</summary>
        public CSharpClassStyle[] ClassStyles { get; } = Enum.GetNames(typeof(CSharpClassStyle))
            .Select(t => (CSharpClassStyle)Enum.Parse(typeof(CSharpClassStyle), t))
            .ToArray();

        /// <summary>Gets the list of class styles.</summary>
        public CSharpControllerStyle[] ControllerStyles { get; } = Enum.GetNames(typeof(CSharpControllerStyle))
            .Select(t => (CSharpControllerStyle)Enum.Parse(typeof(CSharpControllerStyle), t))
            .ToArray();

        /// <summary>Gets the list of class targets.</summary>
        public CSharpControllerTarget[] ControllerTargets { get; } = Enum.GetNames(typeof(CSharpControllerTarget))
            .Select(t => (CSharpControllerTarget)Enum.Parse(typeof(CSharpControllerTarget), t))
            .ToArray();

        /// <summary>Gets the list of route naming strategies.</summary>
        public CSharpControllerRouteNamingStrategy[] RouteNamingStrategies { get; } = Enum.GetNames(typeof(CSharpControllerRouteNamingStrategy))
            .Select(t => (CSharpControllerRouteNamingStrategy)Enum.Parse(typeof(CSharpControllerRouteNamingStrategy), t))
            .ToArray();

        /// <summary>Gets new line behaviors. </summary>
        public NewLineBehavior[] NewLineBehaviors { get; } = Enum.GetNames(typeof(NewLineBehavior))
            .Select(t => (NewLineBehavior)Enum.Parse(typeof(NewLineBehavior), t))
            .ToArray();

        /// <summary>Gets or sets the client code.</summary>
        public string ClientCode
        {
            get { return _clientCode; }
            set { Set(ref _clientCode, value); }
        }
    }
}
