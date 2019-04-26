//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration
{
    /// <summary>The client generator base.</summary>
    /// <typeparam name="TOperationModel">The type of the operation model.</typeparam>
    /// <typeparam name="TParameterModel">The type of the parameter model.</typeparam>
    /// <typeparam name="TResponseModel">The type of the response model.</typeparam>
    /// <seealso cref="IClientGenerator" />
    public abstract class ClientGeneratorBase<TOperationModel, TParameterModel, TResponseModel> : IClientGenerator
        where TOperationModel : OperationModelBase<TParameterModel, TResponseModel>
        where TResponseModel : ResponseModelBase
        where TParameterModel : ParameterModelBase
    {
        /// <summary>Initializes a new instance of the <see cref="ClientGeneratorBase{TOperationModel, TParameterModel, TResponseModel}"/> class.</summary>
        /// <param name="document">The document.</param>
        /// <param name="codeGeneratorSettings">The code generator settings.</param>
        /// <param name="resolver">The type resolver.</param>
        protected ClientGeneratorBase(SwaggerDocument document, CodeGeneratorSettingsBase codeGeneratorSettings, TypeResolverBase resolver)
        {
            Resolver = resolver;
            codeGeneratorSettings.SchemaType = document.SchemaType; // enforce Swagger schema output 
        }

        /// <summary>Generates the the whole file containing all needed types.</summary>
        /// <returns>The code</returns>
        public abstract string GenerateFile();

        /// <summary>Gets the file response type name.</summary>
        /// <returns>The type name.</returns>
        public virtual string GetBinaryResponseTypeName()
        {
            return "FileResponse";
        }

        /// <summary>Gets the base settings.</summary>
        public abstract ClientGeneratorBaseSettings BaseSettings { get; }

        /// <summary>Gets the type.</summary>
        /// <param name="schema">The schema.</param>
        /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
        /// <param name="typeNameHint">The type name hint.</param>
        /// <returns>The type name.</returns>
        public abstract string GetTypeName(JsonSchema4 schema, bool isNullable, string typeNameHint);

        /// <summary>Gets the type resolver.</summary>
        protected TypeResolverBase Resolver { get; }

        /// <summary>Generates the file.</summary>
        /// <param name="clientCode">The client code.</param>
        /// <param name="clientClasses">The client classes.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <returns>The code.</returns>
        protected abstract string GenerateFile(string clientCode, IEnumerable<string> clientClasses, ClientGeneratorOutputType outputType);

        /// <summary>Generates the client class.</summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="controllerClassName">Name of the controller class.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <returns>The code.</returns>
        protected abstract string GenerateClientClass(string controllerName, string controllerClassName, IList<TOperationModel> operations, ClientGeneratorOutputType outputType);

        /// <summary>Creates an operation model.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The operation model.</returns>
        protected abstract TOperationModel CreateOperationModel(SwaggerOperation operation, ClientGeneratorBaseSettings settings);

        /// <summary>Generates the file.</summary>
        /// <param name="document">The document.</param>
        /// <param name="type">The type.</param>
        /// <returns>The code.</returns>
        protected string GenerateFile(SwaggerDocument document, ClientGeneratorOutputType type)
        {
            var clientCode = string.Empty;
            var operations = GetOperations(document);
            var clientClasses = new List<string>();

            if (BaseSettings.OperationNameGenerator.SupportsMultipleClients)
            {
                foreach (var controllerOperations in operations.GroupBy(o => o.ControllerName))
                {
                    var controllerName = controllerOperations.Key;
                    var controllerClassName = BaseSettings.GenerateControllerName(controllerOperations.Key);
                    clientCode += GenerateClientClass(controllerName, controllerClassName, controllerOperations.ToList(), type) + "\n\n";
                    clientClasses.Add(controllerClassName);
                }
            }
            else
            {
                var controllerName = string.Empty;
                var controllerClassName = BaseSettings.GenerateControllerName(controllerName);
                clientCode = GenerateClientClass(controllerName, controllerClassName, operations, type);
                clientClasses.Add(controllerClassName);
            }

            return GenerateFile(clientCode, clientClasses, type)
                .Replace("\r", string.Empty)
                .Replace("\n\n\n\n", "\n\n")
                .Replace("\n\n\n", "\n\n");
        }

        private List<TOperationModel> GetOperations(SwaggerDocument document)
        {
            document.GenerateOperationIds();

            return document.Paths
                .SelectMany(pair => pair.Value.Select(p => new { Path = pair.Key.TrimStart('/'), HttpMethod = p.Key, Operation = p.Value }))
                .Select(tuple =>
                {
                    var operationName = BaseSettings.OperationNameGenerator.GetOperationName(document, tuple.Path, tuple.HttpMethod, tuple.Operation);
                    if (operationName.EndsWith("Async"))
                    {
                        operationName = operationName.Substring(0, operationName.Length - "Async".Length);
                    }

                    var operationModel = CreateOperationModel(tuple.Operation, BaseSettings);
                    operationModel.ControllerName = BaseSettings.OperationNameGenerator.GetClientName(document, tuple.Path, tuple.HttpMethod, tuple.Operation);
                    operationModel.Path = tuple.Path;
                    operationModel.HttpMethod = tuple.HttpMethod;
                    operationModel.OperationName = operationName;
                    return operationModel;
                })
                .ToList();
        }
    }
}