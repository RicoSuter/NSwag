using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace NSwag.Commands
{
    internal class BaseTypeMappingContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly IDictionary<Type, Type> _mappings;

        public BaseTypeMappingContractResolver(IDictionary<Type, Type> mappings)
        {
            _mappings = mappings;
        }

        public override JsonContract ResolveContract(Type type)
        {
            return base.ResolveContract(_mappings != null && _mappings.ContainsKey(type) ? _mappings[type] : type);
        }
    }
}