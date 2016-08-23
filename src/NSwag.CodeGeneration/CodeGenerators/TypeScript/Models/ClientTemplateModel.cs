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
    /// <summary>This is base model used in TypeScript client generation process. It is passed to T4 template.</summary>
    public class ClientTemplateModel
    {
        /// <summary>Initializes a new instance of the <see cref="ClientTemplateModel" /> class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="service">The service.</param>
        /// <param name="settings">The settings.</param>
        public ClientTemplateModel(string controllerName, IList<OperationModel> operations, SwaggerService service, SwaggerToTypeScriptClientGeneratorSettings settings)
        {
            Class = controllerName;
            IsExtended = settings.TypeScriptGeneratorSettings.ExtendedClasses?.Any(c => c + "Base" == controllerName) == true;

            HasOperations = operations.Any();
            Operations = operations;
            UsesKnockout = settings.TypeScriptGeneratorSettings.TypeStyle == TypeScriptTypeStyle.KnockoutClass;

            BaseUrl = service.BaseUrl;
            GenerateClientInterfaces = settings.GenerateClientInterfaces;

            PromiseType = settings.PromiseType == TypeScript.PromiseType.Promise ? "Promise" : "Q.Promise";
            PromiseConstructor = settings.PromiseType == TypeScript.PromiseType.Promise ? "new Promise" : "Q.Promise";
        }

        /// <summary>Gets the class name.</summary>
        public string Class { get; }

        /// <summary>Gets a value indicating whether the client is extended with an extension class.</summary>
        public bool IsExtended { get; }

        /// <summary>Gets a value indicating whether the client has operations.</summary>
        public bool HasOperations { get; }

        /// <summary>Gets the operations.</summary>
        public IList<OperationModel> Operations { get; }

        /// <summary>Gets a value indicating whether the client uses KnockoutJS.</summary>
        public bool UsesKnockout { get; }

        /// <summary>Gets the service base URL.</summary>
        public string BaseUrl { get; }

        /// <summary>Gets a value indicating whether to generate client interfaces.</summary>
        public bool GenerateClientInterfaces { get; }

        /// <summary>Gets the promise type.</summary>
        public string PromiseType { get; }

        /// <summary>Gets the promise constructor code.</summary>
        public string PromiseConstructor { get; }
    }
}