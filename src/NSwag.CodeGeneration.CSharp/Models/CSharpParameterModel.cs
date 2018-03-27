//-----------------------------------------------------------------------
// <copyright file="CSharpParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

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
        /// <param name="typeName">The type name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        public CSharpParameterModel(string parameterName, string variableName, string typeName, SwaggerParameter parameter,
            IList<SwaggerParameter> allParameters, CodeGeneratorSettingsBase settings, IClientGenerator generator)
            : base(parameterName, variableName, typeName, parameter, allParameters, settings, generator,
                  new CSharpValueGenerator(new CSharpTypeResolver((CSharpGeneratorSettings)settings), (CSharpGeneratorSettings)settings))
        {
        }

        /// <summary>Gets a value indicating whether the type is a Nullable&lt;&gt;.</summary>
        public bool IsSystemNullable => Type.EndsWith("?");

        /// <summary>Gets the type of the parameter when used in a controller interface where we can set default values before calling.</summary>
        public string TypeInControllerInterface => HasDefault ? Type.EndsWith("?") ? Type.Substring(0, Type.Length - 1) : Type : Type;
    }
}
