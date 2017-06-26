//-----------------------------------------------------------------------
// <copyright file="CSharpParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.Models;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp parameter model.</summary>
    public class CSharpParameterModel : ParameterModelBase
    {
        private readonly SwaggerToCSharpGeneratorSettings _csharpSettings;
        private readonly CSharpDefaultValueGenerator _defaultValueGenerator;

        /// <summary>Initializes a new instance of the <see cref="CSharpParameterModel" /> class.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="csharpSettings">C# settings.</param>
        /// <param name="generator">The client generator base.</param>
        /// <param name="defaultValueGenerator">The default value generator for C#.</param>
        public CSharpParameterModel(string parameterName, string variableName, string typeName, 
            SwaggerParameter parameter, 
            IList<SwaggerParameter> allParameters, 
            CodeGeneratorSettingsBase settings, 
            SwaggerToCSharpGeneratorSettings csharpSettings, 
            IClientGenerator generator,
            CSharpDefaultValueGenerator defaultValueGenerator)
            : base(parameterName, variableName, typeName, parameter, allParameters, settings, generator)
        {
            _csharpSettings = csharpSettings;
            _defaultValueGenerator = defaultValueGenerator;
        }

        /// <summary>
        /// Gets a default value of the parameter formatted to be used in C# code.
        /// </summary>
        public string DefaultValueCode
        {
            get
            {
                if (!_csharpSettings.GenerateOptionalParameterDefaultValues) return "null";
                return _defaultValueGenerator.GetDefaultValue(Schema, true, Type, Type, true) ?? "null";
            }
        }

    }
}
