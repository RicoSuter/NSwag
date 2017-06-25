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
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>The TypeScript parameter model.</summary>
    public class TypeScriptParameterModel : ParameterModelBase
    {
        private SwaggerToTypeScriptClientGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptParameterModel" /> class.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        public TypeScriptParameterModel(string parameterName, string variableName, string typeName, SwaggerParameter parameter,
            IList<SwaggerParameter> allParameters, SwaggerToTypeScriptClientGeneratorSettings settings, SwaggerToTypeScriptClientGenerator generator)
            : base(parameterName, variableName, typeName, parameter, allParameters, settings.TypeScriptGeneratorSettings, generator)
        {
            _settings = settings;
        }


        /// <summary>
        /// Gets a default value of the parameter formatted to be used in C# code.
        /// </summary>
        public string DefaultValueCode
        {
            get
            {
                if (!_settings.GenerateOptionalParameterDefaultValues) return "";

                if (Schema.Type == JsonObjectType.String && DefaultValue is string)
                {
                    return " = \"" + ConversionUtilities.ConvertToStringLiteral((string)DefaultValue) + "\"";
                }
                if (Schema.Type == JsonObjectType.Integer)
                {
                    if (DefaultValue is byte) return " = " + ((byte)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is sbyte) return " = " + ((sbyte)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is short) return " = " + ((short)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is ushort) return " = " + ((ushort)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is int) return " = " + ((int)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is uint) return " = " + ((uint)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is long) return " = " + ((long)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is ulong) return " = " + ((ulong)DefaultValue).ToString(CultureInfo.InvariantCulture);
                }
                if (Schema.Type == JsonObjectType.Number)
                {
                    if (DefaultValue is float) return " = " + ((float)DefaultValue).ToString("r", CultureInfo.InvariantCulture);
                    if (DefaultValue is double) return " = " + ((double)DefaultValue).ToString("r", CultureInfo.InvariantCulture);
                    if (DefaultValue is decimal) return " = " + ((decimal)DefaultValue).ToString(CultureInfo.InvariantCulture);
                }
                if (Schema.Type == JsonObjectType.Boolean)
                {
                    if (DefaultValue is bool) return " = " + ((bool)DefaultValue).ToString().ToLower();
                }
                return "";
            }
        }
    }
}