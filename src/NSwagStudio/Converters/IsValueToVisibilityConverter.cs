using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace NSwagStudio.Converters
{
    public class IsValueToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Target { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null && Target == null)
                return Visibility.Visible;

            if (value == null || Target == null)
                return Visibility.Collapsed;

            if (value.Equals(Target))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}