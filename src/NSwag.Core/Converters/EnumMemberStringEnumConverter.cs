using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NSwag.Converters
{
    /// <summary>A JSON string enum converter that respects [EnumMember(Value = "...")] attributes
    /// for custom serialization names. Wraps JsonStringEnumConverter with a naming policy.</summary>
    internal class EnumMemberStringEnumConverter<T> : JsonStringEnumConverter<T> where T : struct, Enum
    {
        public EnumMemberStringEnumConverter() : base(new EnumMemberNamingPolicy())
        {
        }

        private sealed class EnumMemberNamingPolicy : JsonNamingPolicy
        {
            private readonly Dictionary<string, string> _nameMap;

            public EnumMemberNamingPolicy()
            {
                _nameMap = new Dictionary<string, string>();
                foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
                    if (attribute?.Value != null)
                    {
                        _nameMap[field.Name] = attribute.Value;
                    }
                }
            }

            public override string ConvertName(string name) =>
                _nameMap.TryGetValue(name, out var mapped) ? mapped : name;
        }
    }
}
