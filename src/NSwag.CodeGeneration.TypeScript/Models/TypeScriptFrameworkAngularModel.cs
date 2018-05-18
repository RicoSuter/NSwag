//-----------------------------------------------------------------------
// <copyright file="TypeScriptFrameworkAngularModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>Angular specific information.</summary>
    public class TypeScriptFrameworkAngularModel
    {
        private readonly SwaggerToTypeScriptClientGeneratorSettings _settings;

        internal TypeScriptFrameworkAngularModel(SwaggerToTypeScriptClientGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Gets or sets the injection token type (used in the Angular template).</summary>
        public string InjectionTokenType => _settings.InjectionTokenType.ToString();

        /// <summary>Gets or sets the token name for injecting the API base URL string (used in the Angular template).</summary>
        public string BaseUrlTokenName => _settings.BaseUrlTokenName;

        /// <summary>Gets a value indicating whether to use HttpClient with the Angular template.</summary>
        public bool UseHttpClient => _settings.HttpClass == TypeScript.HttpClass.HttpClient;

        /// <summary>Gets the HTTP client class name.</summary>
        public string HttpClass => UseHttpClient ? "HttpClient" : "Http";
    }
}