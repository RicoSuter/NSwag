//-----------------------------------------------------------------------
// <copyright file="WebApiAssemblyToSwaggerGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary></summary>
    public abstract class WebApiAssemblyToSwaggerGeneratorBase
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiAssemblyToSwaggerGeneratorBase"/> class.</summary>
        /// <param name="settings">The generator settings.</param>
        protected WebApiAssemblyToSwaggerGeneratorBase(WebApiAssemblyToSwaggerGeneratorSettings settings)
        {
            Settings = settings;
        }

        /// <summary>Gets or sets the settings.</summary>
        public WebApiAssemblyToSwaggerGeneratorSettings Settings { get; protected set; }

        /// <summary>Generates for controllers.</summary>
        /// <param name="controllerClassNames">The controller class names.</param>
        /// <returns>The Swagger specification.</returns>
        public abstract SwaggerService GenerateForControllers(IEnumerable<string> controllerClassNames);

        /// <summary>Gets the controller classes.</summary>
        /// <returns>The controller class names.</returns>
        public abstract string[] GetControllerClasses();
    }
}