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
        /// <summary>Initializes a new instance of the <see cref="CSharpTemplateModelBase"/> class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="settings">The settings.</param>
        protected CSharpTemplateModelBase(string controllerName, SwaggerToCSharpGeneratorSettings settings)
        {
            ResponseClass = settings.ResponseClass.Replace("{controller}", controllerName);
            WrapResponses = settings.WrapResponses;
        }

        /// <summary>Gets a value indicating whether to wrap success responses to allow full response access.</summary>
        public bool WrapResponses { get; set; }

        /// <summary>Gets the response class name.</summary>
        public string ResponseClass { get; protected set; }
    }
}
