//-----------------------------------------------------------------------
// <copyright file="CSharpControllerTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp controller template model.</summary>
    public class CSharpControllerTemplateModel : CSharpTemplateModelBase
    {
        private readonly SwaggerToCSharpWebApiControllerGeneratorSettings _settings;
        private readonly SwaggerDocument _document;

        /// <summary>Initializes a new instance of the <see cref="CSharpControllerTemplateModel" /> class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="document">The document.</param>
        /// <param name="settings">The settings.</param>
        public CSharpControllerTemplateModel(
            string controllerName,
            IEnumerable<CSharpOperationModel> operations,
            SwaggerDocument document,
            SwaggerToCSharpWebApiControllerGeneratorSettings settings)
            : base(controllerName, settings)
        {
            _document = document;
            _settings = settings;

            Class = controllerName;
            Operations = operations;

            BaseClass = _settings.ControllerBaseClass?.Replace("{controller}", controllerName);
        }

        /// <summary>Gets or sets the class name.</summary>
        public string Class { get; }

        /// <summary>Gets a value indicating whether the controller has a base class.</summary>
        public bool HasBaseClass => !string.IsNullOrEmpty(BaseClass);

        /// <summary>Gets the base class.</summary>
        public string BaseClass { get; }

        /// <summary>Gets or sets the service base URL.</summary>
        public string BaseUrl => _document.BaseUrl;

        /// <summary>Gets or sets a value indicating whether the controller has operations.</summary>
        public bool HasOperations => Operations.Any();

        /// <summary>Gets or sets the operations.</summary>
        public IEnumerable<CSharpOperationModel> Operations { get; set; }

        /// <summary>Gets or sets a value indicating whether the controller has a base path.</summary>
        public bool HasBasePath => !string.IsNullOrEmpty(BasePath);

        /// <summary>Gets or sets the base path.</summary>
        public string BasePath => _document.BasePath?.TrimStart('/');

        /// <summary>Gets a value indicating whether to generate optional parameters.</summary>
        public bool GenerateOptionalParameters => _settings.GenerateOptionalParameters;
    }
}