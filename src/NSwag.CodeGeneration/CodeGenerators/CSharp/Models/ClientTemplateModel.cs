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
        public ClientTemplateModel(string controllerName, IList<OperationModel> operations, SwaggerService service, SwaggerToCSharpClientGeneratorSettings settings)
        {
            var hasClientBaseClass = !string.IsNullOrEmpty(settings.ClientBaseClass);

            Class = controllerName;
            BaseClass = settings.ClientBaseClass;

            HasBaseClass = hasClientBaseClass;
            HasBaseType = settings.GenerateClientInterfaces || hasClientBaseClass;

            UseHttpClientCreationMethod = settings.UseHttpClientCreationMethod;
            GenerateClientInterfaces = settings.GenerateClientInterfaces;
            BaseUrl = service.BaseUrl;

            Operations = operations;
        }

        public string Class { get; }

        public string BaseClass { get; }

        public bool HasBaseClass { get; }

        public bool HasBaseType { get; }

        public bool UseHttpClientCreationMethod { get; }

        public bool GenerateClientInterfaces { get; }

        public string BaseUrl { get; }

        public bool HasMissingHttpMethods => Operations.Any(o =>
            o.HttpMethod == SwaggerOperationMethod.Options ||
            o.HttpMethod == SwaggerOperationMethod.Head ||
            o.HttpMethod == SwaggerOperationMethod.Patch);

        public IList<OperationModel> Operations { get; }

        public bool HasOperations => Operations.Any();
    }
}