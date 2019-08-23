//-----------------------------------------------------------------------
// <copyright file="RouteAttributeFacade.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;

namespace NSwag.Generation.WebApi.Infrastructure
{
    /// <summary>
    /// Uses reflection to provide a common interface to the following types:
    /// * RouteAttribute
    /// * IHttpRouteInfoProvider
    /// * IRouteTemplateProvider
    /// </summary>
    internal class RouteAttributeFacade
    {
        private readonly PropertyInfo _template;

        private RouteAttributeFacade(Attribute attr, PropertyInfo template)
        {
            Attribute = attr;
            _template = template;
        }

        public RouteAttributeFacade(Attribute attr)
        {
            var type = attr.GetType();

            _template = type.GetRuntimeProperty("Template");
            if (_template == null)
            {
                throw new ArgumentException($"{type.FullName}  does not implement property 'Template'");
            }

            Attribute = attr;
        }

        public Attribute Attribute { get; }

        public string Template => (string)_template.GetValue(Attribute);

        public static RouteAttributeFacade TryMake(Attribute a)
        {
            var type = a.GetType();
            var typeInfo = type.GetTypeInfo();

            if (type.Name == "RouteAttribute" ||
                typeInfo.ImplementedInterfaces.Any(i => i.Name == "IHttpRouteInfoProvider") ||
                typeInfo.ImplementedInterfaces.Any(i => i.Name == "IRouteTemplateProvider")) // .NET Core
            {
                var template = type.GetRuntimeProperty("Template");
                if (template != null)
                {
                    return new RouteAttributeFacade(a, template);
                }
            }

            return null;
        }
    }
}
