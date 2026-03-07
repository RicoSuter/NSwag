using System.Globalization;
using Avalonia.Data.Converters;

namespace NSwagStudio.Converters;

public class VisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b;
        if (value is string s)
            return !string.IsNullOrEmpty(s);
        return value != null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b;
        return false;
    }
}
