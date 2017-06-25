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

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp parameter model.</summary>
    public class CSharpParameterModel : ParameterModelBase
    {
        private readonly SwaggerToCSharpGeneratorSettings _csharpSettings;

        /// <summary>Initializes a new instance of the <see cref="CSharpParameterModel" /> class.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="csharpSettings">C# settings.</param>
        /// <param name="generator">The client generator base.</param>
        public CSharpParameterModel(string parameterName, string variableName, string typeName, 
            SwaggerParameter parameter, 
            IList<SwaggerParameter> allParameters, 
            CodeGeneratorSettingsBase settings, 
            SwaggerToCSharpGeneratorSettings csharpSettings, 
            IClientGenerator generator)
            : base(parameterName, variableName, typeName, parameter, allParameters, settings, generator)
        {
            _csharpSettings = csharpSettings;
        }

        /// <summary>
        /// Gets a default value of the parameter formatted to be used in C# code.
        /// </summary>
        public string DefaultValueCode
        {
            get
            {
                if (!_csharpSettings.GenerateOptionalParameterDefaultValues) return "null";

                if (Schema.Type == JsonObjectType.String && DefaultValue is string)
                {
                    return "\"" + ConversionUtilities.ConvertToStringLiteral((string) DefaultValue) + "\"";
                }
                if (Schema.Type == JsonObjectType.Integer)
                {
                    if (DefaultValue is byte) return "(byte)" + ((byte)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is sbyte) return "(sbyte)" + ((sbyte)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is short) return "(short)" + ((short)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is ushort) return "(ushort)" + ((ushort)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is int) return ((int)DefaultValue).ToString(CultureInfo.InvariantCulture);
                    if (DefaultValue is uint) return ((uint)DefaultValue).ToString(CultureInfo.InvariantCulture) + "U";
                    if (DefaultValue is long) return ((long)DefaultValue).ToString(CultureInfo.InvariantCulture) + "L";
                    if (DefaultValue is ulong) return ((ulong)DefaultValue).ToString(CultureInfo.InvariantCulture) + "UL";
                }
                if (Schema.Type == JsonObjectType.Number)
                {
                    if (DefaultValue is float) return ((float) DefaultValue).ToString("r", CultureInfo.InvariantCulture) + "F";
                    if (DefaultValue is double) return ((double)DefaultValue).ToString("r", CultureInfo.InvariantCulture) + "D";
                    if (DefaultValue is decimal) return ((decimal)DefaultValue).ToString(CultureInfo.InvariantCulture) + "M";
                }
                if (Schema.Type == JsonObjectType.Boolean)
                {
                    if (DefaultValue is bool) return ((bool) DefaultValue).ToString().ToLower();
                }
                return "null";
            }
        }

    }
}
