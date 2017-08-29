//-----------------------------------------------------------------------
// <copyright file="CSharpTemplateModelBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>Base class for the CSharp models</summary>
    public abstract class CSharpTemplateModelBase
    {
        private readonly string _controllerName;
        private readonly SwaggerToCSharpGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="CSharpTemplateModelBase"/> class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="settings">The settings.</param>
        protected CSharpTemplateModelBase(string controllerName, SwaggerToCSharpGeneratorSettings settings)
        {
            _controllerName = controllerName;
            _settings = settings;
        }

        /// <summary>Gets a value indicating whether to wrap success responses to allow full response access.</summary>
        public bool WrapResponses => _settings.WrapResponses;

        /// <summary>Gets the response class name.</summary>
        public string ResponseClass => _settings.ResponseClass.Replace("{controller}", _controllerName);
    }
}
