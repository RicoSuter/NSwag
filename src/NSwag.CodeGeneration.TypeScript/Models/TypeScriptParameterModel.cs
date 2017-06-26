//-----------------------------------------------------------------------
// <copyright file="TypeScriptParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>The TypeScript parameter model.</summary>
    public class TypeScriptParameterModel : ParameterModelBase
    {
        private SwaggerToTypeScriptClientGeneratorSettings _settings;
        private readonly TypeScriptDefaultValueGenerator _defaultValueGenerator;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptParameterModel" /> class.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        /// <param name="defaultValueGenerator">The default value generator for TypeScript.</param>
        public TypeScriptParameterModel(string parameterName, string variableName, string typeName, SwaggerParameter parameter, IList<SwaggerParameter> allParameters, SwaggerToTypeScriptClientGeneratorSettings settings, SwaggerToTypeScriptClientGenerator generator, TypeScriptDefaultValueGenerator defaultValueGenerator)
            : base(parameterName, variableName, typeName, parameter, allParameters, settings.TypeScriptGeneratorSettings, generator)
        {
            _settings = settings;
            _defaultValueGenerator = defaultValueGenerator;
        }


        /// <summary>
        /// Gets a default value of the parameter formatted to be used in C# code.
        /// </summary>
        public string DefaultValueCode
        {
            get
            {
                if (!_settings.GenerateOptionalParameterDefaultValues) return "";

                var defaultValue = _defaultValueGenerator.GetDefaultValue(Schema, true, Type, Type, true);
                if (defaultValue == null)
                {
                    return "";
                }
                return " = " + defaultValue;
            }
        }
    }
}