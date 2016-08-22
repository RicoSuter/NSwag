//-----------------------------------------------------------------------
// <copyright file="ClientTemplateModel.cs" company="NSwag">
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
    /// <summary>
    /// 
    /// </summary>
    public class ClientTemplateModel
    {
        private readonly SwaggerService _service;
        private readonly SwaggerToCSharpClientGeneratorSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientTemplateModel"/> class.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="service">The service.</param>
        /// <param name="settings">The settings.</param>
        public ClientTemplateModel(string controllerName, IList<OperationModel> operations, SwaggerService service, SwaggerToCSharpClientGeneratorSettings settings)
        {
            _service = service;
            _settings = settings;

            Class = controllerName;
            Operations = operations;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [generate contracts].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [generate contracts]; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateContracts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [generate implementation].
        /// </summary>
        /// <value>
        /// <c>true</c> if [generate implementation]; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateImplementation { get; set; }

        /// <summary>
        /// Gets the class.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public string Class { get; }

        /// <summary>
        /// Gets the base class.
        /// </summary>
        /// <value>
        /// The base class.
        /// </value>
        public string BaseClass => _settings.ClientBaseClass;

        /// <summary>
        /// Gets a value indicating whether this instance has base class.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has base class; otherwise, <c>false</c>.
        /// </value>
        public bool HasBaseClass => !string.IsNullOrEmpty(_settings.ClientBaseClass);

        /// <summary>
        /// Gets the configuration class.
        /// </summary>
        /// <value>
        /// The configuration class.
        /// </value>
        public string ConfigurationClass => _settings.ConfigurationClass;

        /// <summary>
        /// Gets a value indicating whether this instance has configuration class.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has configuration class; otherwise, <c>false</c>.
        /// </value>
        public bool HasConfigurationClass => !string.IsNullOrEmpty(_settings.ConfigurationClass);

        /// <summary>
        /// Gets a value indicating whether this instance has base type.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has base type; otherwise, <c>false</c>.
        /// </value>
        public bool HasBaseType => _settings.GenerateClientInterfaces || HasBaseClass;

        /// <summary>
        /// Gets a value indicating whether [use HTTP client creation method].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use HTTP client creation method]; otherwise, <c>false</c>.
        /// </value>
        public bool UseHttpClientCreationMethod => _settings.UseHttpClientCreationMethod;

        /// <summary>
        /// Gets a value indicating whether [generate client interfaces].
        /// </summary>
        /// <value>
        /// <c>true</c> if [generate client interfaces]; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateClientInterfaces => _settings.GenerateClientInterfaces;

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        public string BaseUrl => _service.BaseUrl;

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        public IList<OperationModel> Operations { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has operations.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has operations; otherwise, <c>false</c>.
        /// </value>
        public bool HasOperations => Operations.Any();
    }
}