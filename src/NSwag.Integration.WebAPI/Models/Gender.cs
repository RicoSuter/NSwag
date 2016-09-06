using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag.Integration.WebAPI.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        Male,
        Female
    }
}