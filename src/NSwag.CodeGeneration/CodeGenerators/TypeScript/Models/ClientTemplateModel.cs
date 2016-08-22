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
    /// <summary>
    /// 
    /// </summary>
    public class ClientTemplateModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientTemplateModel"/> class.
        /// </summary>
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

        /// <summary>
        /// Gets the class.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public string Class { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is extended.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is extended; otherwise, <c>false</c>.
        /// </value>
        public bool IsExtended { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has operations.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has operations; otherwise, <c>false</c>.
        /// </value>
        public bool HasOperations { get; }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        public IList<OperationModel> Operations { get; }

        /// <summary>
        /// Gets a value indicating whether [uses knockout].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [uses knockout]; otherwise, <c>false</c>.
        /// </value>
        public bool UsesKnockout { get; }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        public string BaseUrl { get; }

        /// <summary>
        /// Gets a value indicating whether [generate client interfaces].
        /// </summary>
        /// <value>
        /// <c>true</c> if [generate client interfaces]; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateClientInterfaces { get; }

        /// <summary>
        /// Gets the type of the promise.
        /// </summary>
        /// <value>
        /// The type of the promise.
        /// </value>
        public string PromiseType { get; }

        /// <summary>
        /// Gets the promise constructor.
        /// </summary>
        /// <value>
        /// The promise constructor.
        /// </value>
        public string PromiseConstructor { get; }
    }
}