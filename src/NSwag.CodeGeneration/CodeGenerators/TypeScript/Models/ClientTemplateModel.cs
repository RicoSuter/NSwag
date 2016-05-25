//-----------------------------------------------------------------------
// <copyright file="ClientTemplateModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript.Models
{
    internal class ClientTemplateModel
    {
        public ClientTemplateModel(string controllerName, IList<OperationModel> operations, SwaggerService service, SwaggerToTypeScriptClientGeneratorSettings settings)
        {
            Class = controllerName;
            IsExtended = settings.TypeScriptGeneratorSettings.ExtendedClasses?.Any(c => c + "Base" == controllerName) == true;

            HasOperations = operations.Any();
            Operations = operations;
            UsesKnockout = settings.TypeScriptGeneratorSettings.TypeStyle == TypeScriptTypeStyle.KnockoutClass;

            BaseUrl = service.BaseUrl;
            GenerateClientInterfaces = settings.GenerateClientInterfaces;
            UseDtoClasses = settings.TypeScriptGeneratorSettings.TypeStyle != TypeScriptTypeStyle.Interface;

            PromiseType = settings.PromiseType == TypeScript.PromiseType.Promise ? "Promise" : "Q.Promise";
            PromiseConstructor = settings.PromiseType == TypeScript.PromiseType.Promise ? "new Promise" : "Q.Promise";
        }

        public string Class { get; }

        public bool IsExtended { get; }

        public bool HasOperations { get; }

        public IList<OperationModel> Operations { get; }

        public bool UsesKnockout { get; }

        public string BaseUrl { get; }

        public bool GenerateClientInterfaces { get; }

        public bool UseDtoClasses { get; }

        public string PromiseType { get; }

        public string PromiseConstructor { get; }
    }
}