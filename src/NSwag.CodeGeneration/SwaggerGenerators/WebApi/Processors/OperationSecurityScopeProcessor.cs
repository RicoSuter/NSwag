//-----------------------------------------------------------------------
// <copyright file="OperationSecurityScopeProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NJsonSchema;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Generates the OAuth2 security scopes for an operation by reflecting the AuthorizeAttribute attributes.</summary>
    public class OperationSecurityScopeProcessor : IOperationProcessor
    {
        private readonly string _name;

        /// <summary>Initializes a new instance of the <see cref="OperationSecurityScopeProcessor"/> class.</summary>
        /// <param name="name">The security definition name.</param>
        public OperationSecurityScopeProcessor(string name)
        {
            _name = name;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <param name="allOperationDescriptions">All operation descriptions.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(SwaggerOperationDescription operationDescription, MethodInfo methodInfo,
            ISchemaResolver schemaResolver, IList<SwaggerOperationDescription> allOperationDescriptions)
        {
            if (operationDescription.Operation.Security == null)
                operationDescription.Operation.Security = new List<SwaggerSecurityRequirement>();

            var scopes = GetScopes(operationDescription, methodInfo, schemaResolver, allOperationDescriptions);
            operationDescription.Operation.Security.Add(new SwaggerSecurityRequirement
            {
                { _name, scopes }
            });

            return true;
        }

        /// <summary>Gets the security scopes for an operation.</summary>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <param name="allOperationDescriptions">All operation descriptions.</param>
        /// <returns>The scopes.</returns>
        protected virtual IEnumerable<string> GetScopes(SwaggerOperationDescription operationDescription, MethodInfo methodInfo,
            ISchemaResolver schemaResolver, IList<SwaggerOperationDescription> allOperationDescriptions)
        {
            var allAttributes = methodInfo.GetCustomAttributes().Concat(
                methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes());

            var authorizeAttributes = allAttributes.Where(a => a.GetType().Name == "AuthorizeAttribute").ToList();
            if (!authorizeAttributes.Any())
                return Enumerable.Empty<string>();

            return authorizeAttributes
                .SelectMany((dynamic attr) => ((string)attr.Roles).Split(','))
                .Distinct();
        }
    }
}
