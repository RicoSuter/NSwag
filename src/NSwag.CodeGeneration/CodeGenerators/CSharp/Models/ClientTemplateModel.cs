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
    internal class ClientTemplateModel
    {
        private readonly SwaggerService _service;
        private readonly SwaggerToCSharpClientGeneratorSettings _settings;

        public ClientTemplateModel(string controllerName, IList<OperationModel> operations, SwaggerService service, SwaggerToCSharpClientGeneratorSettings settings)
        {
            _service = service;
            _settings = settings;

            Class = controllerName;
            Operations = operations;
        }

        public string Class { get; }

        public string BaseClass => _settings.ClientBaseClass;

        public bool HasBaseClass => !string.IsNullOrEmpty(_settings.ClientBaseClass);

        public bool HasBaseType => _settings.GenerateClientInterfaces || HasBaseClass;

        public bool UseHttpClientCreationMethod => _settings.UseHttpClientCreationMethod;

        public bool GenerateClientInterfaces => _settings.GenerateClientInterfaces;

        public string BaseUrl => _service.BaseUrl;

        public bool HasMissingHttpMethods => Operations.Any(o =>
            o.HttpMethod == SwaggerOperationMethod.Options ||
            o.HttpMethod == SwaggerOperationMethod.Head ||
            o.HttpMethod == SwaggerOperationMethod.Patch);

        public IList<OperationModel> Operations { get; }

        public bool HasOperations => Operations.Any();
    }
}