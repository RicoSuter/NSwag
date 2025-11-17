namespace NSwag;

internal static class Polyfills
{
#if !NETCOREAPP
    public static bool StartsWith(this string str, char c) => str.Length > 0 && str[0] == c;
    public static bool EndsWith(this string str, char c) => str.Length > 0 && str[str.Length - 1] == c;
    public static bool Contains(this string str, string s, StringComparison comparison) => str.IndexOf(s, comparison) != -1;
#endif
}