namespace NSwag.Generation
{
    internal static class GenericResultWrapperTypes
    {
        private static bool IsGenericWrapperType(string typeName) => typeName is "Task`1" or "ValueTask`1" or "JsonResult`1" or "ActionResult`1";

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