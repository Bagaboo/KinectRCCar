using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KinectRCCar.WpfUI.Converters
{
	[ValueConversion(typeof(bool), typeof(Brush))]
	public class BoolToLimeGreenWhiteColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool isTrue = (bool) value;

			return isTrue ? Brushes.LimeGreen : Brushes.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SolidColorBrush color = (SolidColorBrush)value;

			return color == Brushes.LimeGreen;
		}
	}
}
