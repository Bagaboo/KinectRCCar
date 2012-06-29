using System;
using System.Globalization;
using System.Windows.Data;

namespace KinectRCCar.UserControls.Converters
{
	[ValueConversion(typeof(double), typeof(string))]
	public class IntToDegreeIntStringConverter : IValueConverter
	{
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int intValue = (int) value;

			return string.Format("{0}°", intValue);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string stringValue = (string) value;

			return int.Parse((stringValue.Substring(0, stringValue.Length - 1)));
		}
	}
}
