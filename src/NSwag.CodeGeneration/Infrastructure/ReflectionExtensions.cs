using System;

namespace NSwag.CodeGeneration.Infrastructure
{
    internal static class ReflectionExtensions
    {
        public static bool InheritsFrom(this Type type, string typeName)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.Name == typeName)
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}
