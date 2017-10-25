using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace NSwagStudio.Converters
{
    public class StringArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string seperator = "\n";
            if (parameter != null)
            {
                seperator = (string)parameter;
            }
            return value != null ? string.Join(seperator, (string[])value) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            char seperator = '\n';
            if (parameter != null)
            {
                seperator = System.Convert.ToChar(parameter);
            }
            return value?.ToString()
                .Trim('\r')
                .Split(seperator)
                .Select(s => s.Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .ToArray() ?? new string[] { };
        }
    }
}