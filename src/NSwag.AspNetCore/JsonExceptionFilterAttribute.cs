//-----------------------------------------------------------------------
// <copyright file="JsonExceptionFilterAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NSwag.Annotations;
using NSwag.Annotations.Converters;

namespace NSwag.AspNetCore
{
    /// <summary>Handles thrown exceptions from action methods and serializes them with the correct HTTP status code.</summary>
    public class JsonExceptionFilterAttribute : ActionFilterAttribute
    {
        private readonly bool _hideStackTrace;
        private readonly IDictionary<string, Assembly> _searchedNamespaces;

        /// <summary> Initializes a new instance of the <see cref="JsonExceptionFilterAttribute"/> class.</summary>
        public JsonExceptionFilterAttribute()
            : this(true, new Dictionary<string, Assembly>())
        {
        }

        /// <summary> Initializes a new instance of the <see cref="JsonExceptionFilterAttribute"/> class.</summary>
        /// <param name="hideStackTrace">If set to <c>true</c> the serializer hides stack trace (i.e. sets the StackTrace to 'HIDDEN').</param>
        public JsonExceptionFilterAttribute(bool hideStackTrace)
            : this(hideStackTrace, new Dictionary<string, Assembly>())
        {
        }

        /// <summary> Initializes a new instance of the <see cref="JsonExceptionFilterAttribute"/> class.</summary>
        /// <param name="hideStackTrace">If set to <c>true</c> the serializer hides stack trace (i.e. sets the StackTrace to 'HIDDEN').</param>
        /// <param name="searchedNamespaces">The namespaces and assemblies to search for exception types.</param>
        public JsonExceptionFilterAttribute(bool hideStackTrace, IDictionary<string, Assembly> searchedNamespaces)
        {
            _hideStackTrace = hideStackTrace;
            _searchedNamespaces = searchedNamespaces;
        }

        /// <summary>Occurs after the action method is invoked.</summary>
        /// <param name="context">The action executed context.</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var json = JsonConvert.SerializeObject(context.Exception, Formatting.None, new JsonExceptionConverter(_hideStackTrace, _searchedNamespaces));

                context.Result = new ContentResult
                {
                    StatusCode = GetStatusCode(context.Exception, context),
                    Content = json,
                    ContentType = "application/json"
                };
            }
            else
                base.OnActionExecuted(context);
        }

        private int GetStatusCode(Exception exception, ActionExecutedContext context)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                var methodInfo = controllerActionDescriptor.MethodInfo;
                var exceptionType = exception.GetType();

                var producesResponseTypeAttributes = methodInfo.GetCustomAttributes<ProducesResponseTypeAttribute>(true);
                var producesResponseTypeAttribute = producesResponseTypeAttributes.FirstOrDefault(a => exceptionType.IsInstanceOfType(a.Type));
                if (producesResponseTypeAttribute != null)
                    return producesResponseTypeAttribute.StatusCode;

                var responseTypeAttributes = methodInfo.GetCustomAttributes().OfType<ResponseTypeAttribute>();
                var responseTypeAttribute = responseTypeAttributes.FirstOrDefault((dynamic a) => exceptionType.IsAssignableFrom((Type)a.ResponseType));
                if (responseTypeAttribute != null)
                {
                    var statusCode = 0;
                    if (int.TryParse(responseTypeAttribute.HttpStatusCode, out statusCode))
                        return statusCode;
                }
            }
            return 500;
        }
    }
}