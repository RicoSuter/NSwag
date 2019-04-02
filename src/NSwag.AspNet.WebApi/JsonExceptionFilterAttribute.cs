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
        private readonly List<Type> _exceptionTypes = new List<Type>();

        /// <summary>Initializes a new instance of the <see cref="JsonExceptionFilterAttribute"/> class.</summary>
        /// <param name="exceptionTypes">The serialized exception types.</param>
        public JsonExceptionFilterAttribute(params Type[] exceptionTypes)
            : this(true, new Dictionary<string, Assembly>(), exceptionTypes)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonExceptionFilterAttribute"/> class.</summary>
        /// <param name="hideStackTrace">If set to <c>true</c> the serializer hides stack trace (i.e. sets the StackTrace to 'HIDDEN').</param>
        /// <param name="exceptionTypes">The serialized exception types.</param>
        public JsonExceptionFilterAttribute(bool hideStackTrace, params Type[] exceptionTypes)
            : this(hideStackTrace, new Dictionary<string, Assembly>(), exceptionTypes)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="JsonExceptionFilterAttribute"/> class.</summary>
        /// <param name="hideStackTrace">If set to <c>true</c> the serializer hides stack trace (i.e. sets the StackTrace to 'HIDDEN').</param>
        /// <param name="searchedNamespaces">The namespaces and assemblies to search for exception types.</param>
        /// <param name="exceptionTypes">The serialized exception types.</param>
        public JsonExceptionFilterAttribute(bool hideStackTrace, IDictionary<string, Assembly> searchedNamespaces, params Type[] exceptionTypes)
        {
            _hideStackTrace = hideStackTrace;
            _searchedNamespaces = searchedNamespaces;
            _exceptionTypes.AddRange(exceptionTypes);
        }

        /// <summary>Occurs after the action method is invoked.</summary>
        /// <param name="context">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Exception != null && (_exceptionTypes.Count == 0 || _exceptionTypes.Exists(t => t.IsInstanceOfType(context.Exception))))
            {
                var configuration = context.ActionContext.ControllerContext.Configuration;

                var settings = configuration?.Formatters?.JsonFormatter?.SerializerSettings ?? JsonConvert.DefaultSettings?.Invoke();
                settings = settings != null ? CopySettings(settings) : new JsonSerializerSettings();
                settings.Converters.Add(new JsonExceptionConverter(_hideStackTrace, _searchedNamespaces));

                var json = JsonConvert.SerializeObject(context.Exception, settings);
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
            if (context.ActionContext.ActionDescriptor is ReflectedHttpActionDescriptor controllerActionDescriptor)
            {
                var methodInfo = controllerActionDescriptor.MethodInfo;
                var exceptionType = exception.GetType();

#pragma warning disable 618
                var statusCodeString = methodInfo.GetCustomAttributes(true).OfType<ResponseTypeAttribute>()
                    .FirstOrDefault(a => a.ResponseType.IsAssignableFrom(exceptionType.GetTypeInfo()))?.HttpStatusCode;

                if (statusCodeString == null)
                {
                    var swaggerResponseAttributes = methodInfo.GetCustomAttributes(true).OfType<SwaggerResponseAttribute>();
                    statusCodeString = swaggerResponseAttributes.FirstOrDefault(a => exceptionType.IsAssignableFrom(a.Type))?.StatusCode;
                }

                if (statusCodeString != null && int.TryParse(statusCodeString, out var statusCode))
                {
                    return statusCode;
                }
            }

            return 500;
        }

        private JsonSerializerSettings CopySettings(JsonSerializerSettings settings)
        {
            var settingsCopy = new JsonSerializerSettings();

            foreach (var property in typeof(JsonSerializerSettings).GetRuntimeProperties())
            {
                property.SetValue(settingsCopy, property.GetValue(settings));
            }

            foreach (var converter in settings.Converters)
            {
                settingsCopy.Converters.Add(converter);
            }

            return settingsCopy;
        }
    }
}