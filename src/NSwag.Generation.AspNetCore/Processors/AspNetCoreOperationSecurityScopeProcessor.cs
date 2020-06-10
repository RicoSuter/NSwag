//-----------------------------------------------------------------------
// <copyright file="AspNetCoreOperationSecurityScopeProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSwag.Generation.Processors.Contexts;
using Microsoft.AspNetCore.Authorization;
using NSwag.Generation.AspNetCore;
using NJsonSchema.Infrastructure;
using Namotion.Reflection;

namespace NSwag.Generation.Processors.Security
{
    /// <summary>Generates the OAuth2 security scopes for an operation by reflecting the AuthorizeAttribute attributes.</summary>
    public class AspNetCoreOperationSecurityScopeProcessor : IOperationProcessor
    {
        private readonly string _name;

        /// <summary>Initializes a new instance of the <see cref="OperationSecurityScopeProcessor"/> class with 'Bearer' name.</summary>
        public AspNetCoreOperationSecurityScopeProcessor() : this("Bearer")
        {
        }

        /// <summary>Initializes a new instance of the <see cref="OperationSecurityScopeProcessor"/> class.</summary>
        /// <param name="name">The security definition name.</param>
        public AspNetCoreOperationSecurityScopeProcessor(string name)
        {
            _name = name;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext context)
        {
            var aspNetCoreContext = (AspNetCoreOperationProcessorContext)context;

            var endpointMetadata = aspNetCoreContext?.ApiDescription?.ActionDescriptor?.TryGetPropertyValue<IList<object>>("EndpointMetadata");
            if (endpointMetadata != null)
            {
                var allowAnonymous = endpointMetadata.OfType<AllowAnonymousAttribute>().Any();
                if (allowAnonymous)
                {
                    return true;
                }

                var authorizeAttributes = endpointMetadata.OfType<AuthorizeAttribute>().ToList();
                if (!authorizeAttributes.Any())
                {
                    return true;
                }

                if (context.OperationDescription.Operation.Security == null)
                {
                    context.OperationDescription.Operation.Security = new List<OpenApiSecurityRequirement>();
                }

                var scopes = GetScopes(authorizeAttributes);
                context.OperationDescription.Operation.Security.Add(new OpenApiSecurityRequirement
                {
                    { _name, scopes }
                });
            }

            return true;
        }

        /// <summary>Gets the security scopes for an operation.</summary>
        /// <param name="authorizeAttributes">The authorize attributes.</param>
        /// <returns>The scopes.</returns>
        protected virtual IEnumerable<string> GetScopes(IEnumerable<AuthorizeAttribute> authorizeAttributes)
        {
            return authorizeAttributes
                .Where(a => a.Roles != null)
                .SelectMany(a => a.Roles.Split(','))
                .Distinct();
        }
    }
}
