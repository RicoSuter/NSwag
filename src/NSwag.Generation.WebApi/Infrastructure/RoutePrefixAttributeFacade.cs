//-----------------------------------------------------------------------
// <copyright file="RoutePrefixAttributeFacade.cs" company="NSwag">
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
    /// * RoutePrefixAttribute
    /// * IRoutePrefix
    /// </summary>
    internal class RoutePrefixAttributeFacade
    {
        private readonly PropertyInfo _prefix;

        public RoutePrefixAttributeFacade(Attribute attr)
        {
            var type = attr.GetType();

            _prefix = type.GetRuntimeProperty("Prefix");
            if (_prefix == null)
            {
                throw new ArgumentException($"{type.FullName} does not implement property 'Prefix'");
            }

            Attribute = attr;
        }

        public Attribute Attribute { get; }

        public string Prefix => (string)_prefix.GetValue(Attribute);

        public static RoutePrefixAttributeFacade TryMake(Attribute a)
        {
            var type = a.GetType();

            if (type.Name == "RoutePrefixAttribute" ||
                type.GetTypeInfo().ImplementedInterfaces.Any(i => i.Name == "IRoutePrefix"))
            {
                return new RoutePrefixAttributeFacade(a);
            }

            return null;
        }
    }
}
