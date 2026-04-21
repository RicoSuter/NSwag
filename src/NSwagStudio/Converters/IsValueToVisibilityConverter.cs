using System.Globalization;
using Avalonia.Data.Converters;

namespace NSwagStudio.Converters;

public class IsValueToVisibilityConverter : IValueConverter
{
    public object? Target { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null && Target == null)
            return true;

        if (value == null || Target == null)
            return false;

        if (value.Equals(Target))
            return true;

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
