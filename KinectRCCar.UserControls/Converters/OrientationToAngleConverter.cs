using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace KinectRCCar.UserControls.Converters
{
	[ValueConversion(typeof(Orientation), typeof(double))]
	public class OrientationToAngleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Orientation orientation = (Orientation) value;
			switch (orientation)
			{
				case Orientation.Horizontal:
					return 0.0;
				case Orientation.Vertical:
					return -90.0;
				default:
					throw new NotImplementedException("No Angle conversion is implemented for the given orientation.");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
