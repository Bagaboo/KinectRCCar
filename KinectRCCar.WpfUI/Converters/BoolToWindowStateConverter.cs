using System;
using System.Globalization;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace KinectRCCar.WpfUI.Converters
{
	[ValueConversion(typeof(bool), typeof(WindowState))]
	public class BoolToWindowStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool boolValue = (bool) value;

			return boolValue ? WindowState.Open : WindowState.Closed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			WindowState windowStateValue = (WindowState) value;

			return windowStateValue == WindowState.Open;
		}
	}
}
