using System.Globalization;
using Avalonia.Data.Converters;

namespace NSwagStudio.Converters;

public class EqualityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter == null)
            return value != null;

        var paramStr = parameter.ToString()!;
        bool invert = false;
        if (paramStr.StartsWith("!"))
        {
            invert = true;
            paramStr = paramStr.Substring(1);
        }

        var targets = paramStr.Split(',');
        var valueStr = value?.ToString() ?? "";
        var result = targets.Any(t => string.Equals(t.Trim(), valueStr, StringComparison.OrdinalIgnoreCase));

        return invert ? !result : result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
