using System.Globalization;
using Avalonia.Data.Converters;

namespace NSwagStudio.Converters;

public class StringArrayConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string separator = "\n";
        if (parameter != null)
            separator = (string)parameter;
        return value != null ? string.Join(separator, (string[])value) : string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        char separator = '\n';
        if (parameter != null)
            separator = System.Convert.ToChar(parameter);
        return value?.ToString()
            ?.Trim('\r')
            .Split(separator)
            .Select(s => s.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .ToArray() ?? Array.Empty<string>();
    }
}
