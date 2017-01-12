//-----------------------------------------------------------------------
// <copyright file="ControllerTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp.Models
{
    /// <summary>The CSharp controller template model.</summary>
    public class ControllerTemplateModel
    {
        private readonly SwaggerToCSharpWebApiControllerGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="ControllerTemplateModel"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public ControllerTemplateModel(SwaggerToCSharpWebApiControllerGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Gets or sets the class name.</summary>
        public string Class { get; set; }

        /// <summary>Gets a value indicating whether the controller has a base class.</summary>
        public bool HasBaseClass => !string.IsNullOrEmpty(_settings.ControllerBaseClass);

        /// <summary>Gets the base class.</summary>
        public string BaseClass => _settings.ControllerBaseClass;

        /// <summary>Gets or sets the service base URL.</summary>
        public string BaseUrl { get; set; }

        /// <summary>Gets or sets a value indicating whether the controller has operations.</summary>
        public bool HasOperations => Operations.Any();

        /// <summary>Gets or sets the operations.</summary>
        public IList<OperationModel> Operations { get; set; }
        
        /// <summary>Gets or sets a value indicating whether the controller has a base path.</summary>
        public bool HasBasePath { get; set; }

        /// <summary>Gets or sets the base path.</summary>
        public string BasePath { get; set; }
    }
}