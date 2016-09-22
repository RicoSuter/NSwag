//-----------------------------------------------------------------------
// <copyright file="JsonExceptionFilterAttribute.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.Annotations;

namespace NSwag.AspNetCore
{
    /// <summary>Handles thrown exceptions from action methods and serializes them with the correct HTTP status code.</summary>
    public class JsonExceptionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var json = JsonConvert.SerializeObject(context.Exception, Formatting.None, new JsonExceptionConverter());

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
                var responseTypeAttribute = responseTypeAttributes.FirstOrDefault((dynamic a) => exceptionType.IsInstanceOfType((Type)a.ResponseType));
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