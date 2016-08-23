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
    /// <summary>The CSharp client template model.</summary>
    public class ClientTemplateModel
    {
        private readonly SwaggerService _service;
        private readonly SwaggerToCSharpClientGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="ClientTemplateModel" /> class.</summary>
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

        /// <summary>Gets or sets a value indicating whether to generate client contracts (i.e. client interfaces).</summary>
        public bool GenerateContracts { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate implementation classes.</summary>
        public bool GenerateImplementation { get; set; }

        /// <summary>Gets the class name.</summary>
        public string Class { get; }

        /// <summary>Gets the base class name.</summary>
        public string BaseClass => _settings.ClientBaseClass;

        /// <summary>Gets a value indicating whether the client has a base class.</summary>
        public bool HasBaseClass => !string.IsNullOrEmpty(_settings.ClientBaseClass);

        /// <summary>Gets a value indicating whether the client has configuration class.</summary>
        public bool HasConfigurationClass => !string.IsNullOrEmpty(_settings.ConfigurationClass);

        /// <summary>Gets the configuration class name.</summary>
        public string ConfigurationClass => _settings.ConfigurationClass;

        /// <summary>Gets a value indicating whether the client has a base type.</summary>
        public bool HasBaseType => _settings.GenerateClientInterfaces || HasBaseClass;

        /// <summary>Gets a value indicating whether to use a HTTP client creation method.</summary>
        public bool UseHttpClientCreationMethod => _settings.UseHttpClientCreationMethod;

        /// <summary>Gets a value indicating whether to generate client interfaces.</summary>
        public bool GenerateClientInterfaces => _settings.GenerateClientInterfaces;

        /// <summary>Gets the service base URL.</summary>
        public string BaseUrl => _service.BaseUrl;

        /// <summary>Gets a value indicating whether the client has operations.</summary>
        public bool HasOperations => Operations.Any();

        /// <summary>Gets the operations.</summary>
        public IList<OperationModel> Operations { get; }
    }
}