using System;
namespace NSwag.Generation
{
    internal static class GenericResultWrapperTypes
    {
        internal static bool IsGenericWrapperType(string typeName) =>
            typeName == "Task`1" ||
            typeName == "ValueTask`1" ||
            typeName == "JsonResult`1" ||
            typeName == "ActionResult`1";

        internal static T RemoveGenericWrapperTypes<T>(T type, Func<T, string> getName, Func<T, T> unwrap)
        {
            // We iterate because a common signature is public async Task<ActionResult<T>> Action()
            while (IsGenericWrapperType(getName(type)))
            {
                type = unwrap(type);
            }

            return type;
        }
    }
}
