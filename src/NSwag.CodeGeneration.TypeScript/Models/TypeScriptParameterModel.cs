//-----------------------------------------------------------------------
// <copyright file="TypeScriptParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>The TypeScript parameter model.</summary>
    public class TypeScriptParameterModel : ParameterModelBase
    {
        private readonly TypeScriptClientGeneratorSettings _settings;
        private readonly JsonSchema _schema;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptParameterModel" /> class.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public TypeScriptParameterModel(
            string parameterName,
            string variableName,
            string typeName,
            OpenApiParameter parameter,
            IList<OpenApiParameter> allParameters,
            TypeScriptClientGeneratorSettings settings,
            TypeScriptClientGenerator generator,
            TypeResolverBase typeResolver)
            : base(parameterName, variableName, typeName, parameter, allParameters, settings.TypeScriptGeneratorSettings, generator, typeResolver)
        {
            _settings = settings;
            _schema = parameter.Schema;
        }

        /// <summary>Gets the type postfix (e.g. ' | null | undefined')</summary>
        public string TypePostfix
        {
            get
            {
                if (_settings.TypeScriptGeneratorSettings.SupportsStrictNullChecks)
                {
                    return (IsNullable == true ? " | null" : "") + (IsRequired == false ? " | undefined" : "");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Format the datetime to a string based on the chosen datetime type setting
        /// </summary>
        public string GetDateTimeToString
        {
            get
            {
                switch (_settings.TypeScriptGeneratorSettings.DateTimeType)
                {
                    case TypeScriptDateTimeType.Date:
                        return "toISOString()";

                    case TypeScriptDateTimeType.MomentJS:
                    case TypeScriptDateTimeType.OffsetMomentJS:
                        if (_schema.Format == JsonFormatStrings.TimeSpan)
                        {
                            return "format('d.hh:mm:ss.SS', { trim: false })";
                        }

                        if (_settings.TypeScriptGeneratorSettings.DateTimeType == TypeScriptDateTimeType.OffsetMomentJS)
                        {
                            return "toISOString(true)";
                        }
                        return "toISOString()";

                    case TypeScriptDateTimeType.String:
                        return "";

                    case TypeScriptDateTimeType.Luxon:
                        return "toString()";

                    case TypeScriptDateTimeType.DayJS:
                        if (_schema.Format == JsonFormatStrings.TimeSpan)
                        {
                            return "format('d.hh:mm:ss.SSS')";
                        }

                        return "toISOString()";

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}
