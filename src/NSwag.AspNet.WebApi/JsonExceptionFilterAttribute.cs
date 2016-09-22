//-----------------------------------------------------------------------
// <copyright file="JsonExceptionFilterAttribute.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using NJsonSchema.Converters;
using NSwag.Annotations;

namespace NSwag.AspNet.WebApi
{
    /// <summary>Handles thrown exceptions from action methods and serializes them with the correct HTTP status code.</summary>
    public class JsonExceptionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>Occurs after the action method is invoked.</summary>
        /// <param name="context">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var json = JsonConvert.SerializeObject(context.Exception, Formatting.None, new JsonExceptionConverter());
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