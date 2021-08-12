﻿//-----------------------------------------------------------------------
// <copyright file="CSharpParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp parameter model.</summary>
    public class CSharpParameterModel : ParameterModelBase
    {
        /// <summary>Initializes a new instance of the <see cref="CSharpParameterModel" /> class.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="variableIdentifier">Identifier of the variable.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public CSharpParameterModel(
            string parameterName,
            string variableName,
            string variableIdentifier,
            string typeName,
            OpenApiParameter parameter,
            IList<OpenApiParameter> allParameters,
            CodeGeneratorSettingsBase settings,
            IClientGenerator generator,
            TypeResolverBase typeResolver)
            : base(parameterName, variableName, typeName, parameter, allParameters, settings, generator, typeResolver)
        {
            this.VariableIdentifier = variableIdentifier;
        }

        /// <summary>Gets a value indicating whether the type is a Nullable&lt;&gt;.</summary>
        public bool IsSystemNullable => Type.EndsWith("?");

        /// <summary>Gets the type of the parameter when used in a controller interface where we can set default values before calling.</summary>
        public string TypeInControllerInterface => HasDefault ? Type.EndsWith("?") ? Type.Substring(0, Type.Length - 1) : Type : Type;

        /// <summary>Gets a value indicating whether the parameter name is a valid CSharp identifier.</summary>
        public bool IsValidIdentifier => Name.Equals(VariableName, StringComparison.OrdinalIgnoreCase);

        /// <summary>Gets a value indicating whether the parameter allows additional properties.</summary>
        public bool HasAdditionalProperties =>
            IsObject &&
            Schema.AllowAdditionalProperties &&
            !IsDictionary &&
            Type != "object";

        /// <summary>Gets the unescaped variable name.</summary>
        public string VariableIdentifier { get; }
    }
}
