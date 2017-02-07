//-----------------------------------------------------------------------
// <copyright file="TypeScriptParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>The TypeScript parameter model.</summary>
    public class TypeScriptParameterModel : ParameterModelBase
    {
        private readonly TypeScriptTypeResolver _resolver;
        private readonly SwaggerToTypeScriptClientGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptParameterModel" /> class.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        /// <param name="resolver">The resolver.</param>
        public TypeScriptParameterModel(string parameterName, string variableName, string typeName, SwaggerParameter parameter,
            IList<SwaggerParameter> allParameters, SwaggerToTypeScriptClientGeneratorSettings settings,
            SwaggerToTypeScriptClientGenerator generator, TypeScriptTypeResolver resolver)
            : base(parameterName, variableName, typeName, parameter, allParameters, settings.TypeScriptGeneratorSettings, generator)
        {
            _settings = settings;
            _resolver = resolver;
        }

        /// <summary>Gets or sets a value indicating whether to use a DTO class.</summary>
        public bool UseDtoClass
        {
            get
            {
                if (IsDictionary)
                {
                    if (Schema.AdditionalPropertiesSchema != null)
                    {
                        var itemTypeName = _resolver.Resolve(Schema.AdditionalPropertiesSchema, false, string.Empty);
                        return _settings.TypeScriptGeneratorSettings.GetTypeStyle(itemTypeName) !=
                               TypeScriptTypeStyle.Interface && _resolver.HasTypeGenerator(itemTypeName);
                    }
                    else
                        return false;
                }
                else if (IsArray)
                {
                    if (Schema.Item != null)
                    {
                        var itemTypeName = _resolver.Resolve(Schema.Item, false, string.Empty);
                        return _settings.TypeScriptGeneratorSettings.GetTypeStyle(itemTypeName) !=
                               TypeScriptTypeStyle.Interface && _resolver.HasTypeGenerator(itemTypeName);
                    }
                    else
                        return false;
                }
                else
                    return _settings.TypeScriptGeneratorSettings.GetTypeStyle(Type) !=
                           TypeScriptTypeStyle.Interface && _resolver.HasTypeGenerator(Type);
            }
        }
    }
}