//-----------------------------------------------------------------------
// <copyright file="JsonExceptionFilterAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NSwag.Annotations;
using NSwag.Generation.AspNetCore;

namespace NSwag.AspNetCore
{
    /// <summary>Handles thrown exceptions from action methods and serializes them with the correct HTTP status code.</summary>
    public class JsonExceptionFilterAttribute : ActionFilterAttribute
    {
        private readonly bool _hideStackTrace;
        private readonly IDictionary<string, Assembly> _searchedNamespaces;
        private readonly List<Type> _exceptionTypes = [];

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
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null && (_exceptionTypes.Count == 0 || _exceptionTypes.Exists(t => t.IsInstanceOfType(context.Exception))))
            {
                var options = AspNetCoreOpenApiDocumentGenerator.GetSystemTextJsonSettings(context.HttpContext?.RequestServices)
                    ?? new JsonSerializerOptions();

#pragma warning disable CA1869 // options vary per request (from DI)
                var optionsCopy = new JsonSerializerOptions(options)
#pragma warning restore CA1869
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                };

                var json = System.Text.Json.JsonSerializer.Serialize(
                    SerializeException(context.Exception), optionsCopy);

                context.Result = new ContentResult
                {
                    StatusCode = GetStatusCode(context.Exception, context),
                    Content = json,
                    ContentType = "application/json"
                };

                // Required otherwise the framework exception handlers ignores the
                // Result and redirects to a error page or displays in dev mode the stack trace.
                context.ExceptionHandled = true;
            }
            else
            {
                base.OnActionExecuted(context);
            }
        }

        private static int GetStatusCode(Exception exception, ActionExecutedContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var methodInfo = controllerActionDescriptor.MethodInfo;
                var exceptionType = exception.GetType();

                var producesResponseTypeAttribute = methodInfo.GetCustomAttributes<ProducesResponseTypeAttribute>(true)
                    .FirstOrDefault(a => exceptionType.IsInstanceOfType(a.Type));

                if (producesResponseTypeAttribute != null)
                {
                    return producesResponseTypeAttribute.StatusCode;
                }

#pragma warning disable 618
                var statusCodeString = methodInfo.GetCustomAttributes(true).OfType<ResponseTypeAttribute>()
                    .FirstOrDefault(a => exceptionType.IsAssignableFrom(a.ResponseType))?.HttpStatusCode;

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

        private static readonly HashSet<string> _ignoredExceptionProperties =
            ["Message", "StackTrace", "Source", "InnerException", "Data", "TargetSite", "HelpLink", "HResult"];

        private JsonObject SerializeException(Exception exception)
        {
            var jsonObject = new JsonObject
            {
                ["discriminator"] = exception.GetType().Name,
                ["Message"] = exception.Message,
                ["StackTrace"] = _hideStackTrace ? "HIDDEN" : exception.StackTrace,
                ["Source"] = exception.Source,
                ["InnerException"] = exception.InnerException != null
                    ? SerializeException(exception.InnerException)
                    : null
            };

            foreach (var property in exception.GetType().GetRuntimeProperties()
                .Where(p => p.GetMethod?.IsPublic == true && !_ignoredExceptionProperties.Contains(p.Name)))
            {
                var propertyValue = property.GetValue(exception);
                if (propertyValue != null)
                {
                    jsonObject[property.Name] = JsonValue.Create(propertyValue);
                }
            }

            return jsonObject;
        }
    }
}