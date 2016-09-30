//-----------------------------------------------------------------------
// <copyright file="JsonExceptionFilterAttribute.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using NSwag.Annotations;
using NSwag.Annotations.Converters;

namespace NSwag.AspNet.WebApi
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
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var json = JsonConvert.SerializeObject(context.Exception, Formatting.None, new JsonExceptionConverter(_hideStackTrace, _searchedNamespaces));
                context.Response = new HttpResponseMessage
                {
                    StatusCode = (HttpStatusCode)GetStatusCode(context.Exception, context),
                    Content = new StringContent(json, Encoding.UTF8)
                };
            }
            else
                base.OnActionExecuted(context);
        }

        private int GetStatusCode(Exception exception, HttpActionExecutedContext context)
        {
            var controllerActionDescriptor = context.ActionContext.ActionDescriptor as ReflectedHttpActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                var methodInfo = controllerActionDescriptor.MethodInfo;
                var exceptionType = exception.GetType();

                var responseTypeAttributes = methodInfo.GetCustomAttributes(true).OfType<ResponseTypeAttribute>();
                var responseTypeAttribute = responseTypeAttributes.FirstOrDefault(a => a.ResponseType.IsAssignableFrom(exceptionType.GetTypeInfo()));
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