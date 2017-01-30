//-----------------------------------------------------------------------
// <copyright file="CSharpParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp.Models
{
    /// <summary>The CSharp parameter model.</summary>
    public class CSharpParameterModel : ParameterModelBase
    {
        /// <summary>Initializes a new instance of the <see cref="CSharpParameterModel"/> class.</summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        public CSharpParameterModel(string typeName, SwaggerOperation operation, SwaggerParameter parameter, 
            string parameterName, string variableName, CodeGeneratorSettingsBase settings, IClientGenerator generator) 
            : base(typeName, operation, parameter, parameterName, variableName, settings, generator)
        {
        }
    }
}
