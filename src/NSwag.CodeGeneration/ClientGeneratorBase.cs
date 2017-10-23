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
        /// <param name="resolver">The type resolver.</param>
        /// <param name="codeGeneratorSettings">The code generator settings.</param>
        protected ClientGeneratorBase(TypeResolverBase resolver, CodeGeneratorSettingsBase codeGeneratorSettings)
        {
            Resolver = resolver;
            codeGeneratorSettings.SchemaType = SchemaType.Swagger2; // enforce Swagger schema output 
        }

        /// <summary>Generates the the whole file containing all needed types.</summary>
        /// <returns>The code</returns>
        public abstract string GenerateFile();

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

            var operations = document.Paths
                .SelectMany(pair => pair.Value.Select(p => new { Path = pair.Key.Trim('/'), HttpMethod = p.Key, Operation = p.Value }))
                .Select(tuple =>
                {
                    var operationModel = CreateOperationModel(tuple.Operation, BaseSettings);
                    operationModel.ControllerName = BaseSettings.OperationNameGenerator.GetClientName(document, tuple.Path, tuple.HttpMethod, tuple.Operation);
                    operationModel.Path = tuple.Path;
                    operationModel.HttpMethod = tuple.HttpMethod;
                    operationModel.OperationName = BaseSettings.OperationNameGenerator.GetOperationName(document, tuple.Path, tuple.HttpMethod, tuple.Operation);
                    return operationModel;
                })
                .ToList();

            operations = ResolveOperationConflicts(document, operations);

            return operations;
        }

        /// <summary>
        /// Operation model comparer.
        /// </summary>
        internal class OperationComparer : IEqualityComparer<TOperationModel>
        {
            public bool Equals(TOperationModel x, TOperationModel y)
            {
                // Check whether the compared objects reference the same data.
                if (ReferenceEquals(x, y)) return true;

                // Check whether any of the compared objects is null.
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;

                // Check whether the operations' properties are equal.
                return (x.ControllerName == y.ControllerName)
                    && (x.OperationName == y.OperationName)
                    && (x.Parameters.SequenceEqual(y.Parameters, new OperationParameterComparer()));
            }

            public int GetHashCode(TOperationModel operation)
            {
                // Check whether the object is null
                if (ReferenceEquals(operation, null)) return 0;

                // Calculate the hash code.
                var hash = string.IsNullOrEmpty(operation.ControllerName) ? 0 : operation.ControllerName.GetHashCode();
                hash ^= string.IsNullOrEmpty(operation.OperationName) ? 0 : operation.OperationName.GetHashCode();
                foreach (var parameter in operation.Parameters)
                {
                    hash ^= string.IsNullOrEmpty(parameter.Type) ? 0 : parameter.Type.GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// Operation parameter comparer.
        /// </summary>
        internal class OperationParameterComparer : IEqualityComparer<TParameterModel>
        {
            public bool Equals(TParameterModel x, TParameterModel y)
            {
                // Check whether the compared objects reference the same data.
                if (ReferenceEquals(x, y)) return true;

                // Check whether any of the compared objects is null.
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;

                // Check whether the parameters' properties are equal.
                return x.Type == y.Type;
            }

            public int GetHashCode(TParameterModel parameter)
            {
                // Check whether the object is null
                if (ReferenceEquals(parameter, null)) return 0;

                // Get hash code for the Type field if it is not null.
                return string.IsNullOrEmpty(parameter.Type) ? 0 : parameter.Type.GetHashCode();
            }
        }

        /// <summary>
        /// Enumeration of operation name deconfliction suffixes.
        /// </summary>
        internal enum DeconflictSuffix
        {
            /// <summary>
            /// Use underlying HTTP method as name suffix.
            /// </summary>
            HttpMethod,

            /// <summary>
            /// Use operation index as name suffix.
            /// </summary>
            OperationIndex,

            /// <summary>
            /// Placeholder for enumeration end
            /// </summary>
            End
        }

        /// <summary>
        /// Tries to resolve controller operations with overlapping names and signatures.
        /// </summary>
        /// <param name="document">Original Swagger document.</param>
        /// <param name="operations">Operations extracted from Swagger document.</param>
        /// <returns>Deduplicated operations.</returns>
        internal static List<TOperationModel> ResolveOperationConflicts(SwaggerDocument document, List<TOperationModel> operations)
        {
            var ret = operations;
            if (ret == null || ret.Count == 0)
            {
                return ret;
            }
            // Set initial deconfliction suffix
            var suffix = DeconflictSuffix.HttpMethod;
            List<IGrouping<TOperationModel, TOperationModel>> conflicts = null;
            do
            {
                conflicts = Deconflict(ref operations, conflicts, suffix++);
            }
            while (conflicts.Any() && (suffix < DeconflictSuffix.End));

            return ret;
        }

        internal static List<IGrouping<TOperationModel, TOperationModel>> Deconflict(ref List<TOperationModel> operations, List<IGrouping<TOperationModel, TOperationModel>> conflicts, DeconflictSuffix suffix)
        {
            var comparer = new OperationComparer();
            if (conflicts == null)
            {
                conflicts = operations.GroupBy(x => x, comparer).Where(g => g.Count() > 1).ToList();
            }
            if (conflicts.Any())
            {
                foreach (var conflict in conflicts)
                {
                    for (var idx = 0; idx < conflict.Count(); idx++)
                    {
                        var operation = conflict.ElementAt(idx);
                        switch (suffix)
                        {
                            case DeconflictSuffix.HttpMethod:
                                operation.OperationName += CapitalizeFirst(operation.HttpMethod.ToString());
                                break;

                            case DeconflictSuffix.OperationIndex:
                                operation.OperationName += $"{idx + 1}";
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            return operations.GroupBy(x => x, comparer).Where(g => g.Count() > 1).ToList();
        }

        /// <summary>Capitalizes first letter.</summary>
        /// <param name="name">The name to capitalize.</param>
        /// <returns>Capitalized name.</returns>
        internal static string CapitalizeFirst(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            var capitalized = name.ToLower();
            return char.ToUpper(capitalized[0]) + (capitalized.Length > 1 ? capitalized.Substring(1) : "");
        }
    }
}