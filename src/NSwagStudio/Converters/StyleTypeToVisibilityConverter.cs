using NSwag.CodeGeneration.CSharp.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NSwagStudio.Converters
{
	public class StyleTypeToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			CSharpClientGenerationStyle style = (CSharpClientGenerationStyle)value;
			CSharpClientGenerationStyle target;
			Enum.TryParse<CSharpClientGenerationStyle>((string)parameter, out target);

			if (style.Equals(target))
			{
				return Visibility.Visible;
			}
			return Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
