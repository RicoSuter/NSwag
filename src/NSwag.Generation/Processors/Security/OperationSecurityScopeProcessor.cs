﻿//-----------------------------------------------------------------------
// <copyright file="OperationSecurityScopeProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.Processors.Security
{
    /// <summary>Generates the OAuth2 security scopes for an operation by reflecting the AuthorizeAttribute attributes.</summary>
    public class OperationSecurityScopeProcessor : IOperationProcessor
    {
        private readonly string _name;

        /// <summary>Initializes a new instance of the <see cref="OperationSecurityScopeProcessor"/> class with 'Bearer' name.</summary>
        public OperationSecurityScopeProcessor() : this("Bearer")
        {
        }

        /// <summary>Initializes a new instance of the <see cref="OperationSecurityScopeProcessor"/> class.</summary>
        /// <param name="name">The security definition name.</param>
        public OperationSecurityScopeProcessor(string name)
        {
            _name = name;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext context)
        {
            if (context.OperationDescription.Operation.Security == null)
            {
                context.OperationDescription.Operation.Security = [];
            }

            var scopes = GetScopes(context.OperationDescription, context.MethodInfo);
            context.OperationDescription.Operation.Security.Add(new OpenApiSecurityRequirement
            {
                { _name, scopes }
            });

            return true;
        }

        /// <summary>Gets the security scopes for an operation.</summary>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <returns>The scopes.</returns>
        protected virtual IEnumerable<string> GetScopes(OpenApiOperationDescription operationDescription, MethodInfo methodInfo)
        {
            var allAttributes = methodInfo.GetCustomAttributes().Concat(
                methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes());

            var authorizeAttributes = allAttributes.Where(a => a.GetType().Name == "AuthorizeAttribute").ToList();
            if (authorizeAttributes.Count == 0)
            {
                return [];
            }

            return authorizeAttributes
                .Select(a => (dynamic)a)
                .Where(a => a.Roles != null)
                .SelectMany(a => ((string)a.Roles).Split(','))
                .Distinct();
        }
    }
}
