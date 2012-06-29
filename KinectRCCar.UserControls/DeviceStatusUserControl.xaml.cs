using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using KinectRCCar.Infrastructure.Interfaces;

using Microsoft.Kinect;

namespace KinectRCCar.UserControls
{
	/// <summary>
	/// Interaction logic for DeviceStatusUserControl.xaml
	/// </summary>
	public partial class DeviceStatusUserControl : UserControl
	{
		public DeviceStatusUserControl()
		{
			InitializeComponent();
			if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
			{
				CarControllerStatus = CarControllerStatus.Disconnected;
				KinectStatus = KinectStatus.Error;
			}
			
		}

		#region CarControllerStatus
		public static readonly DependencyProperty CarControllerStatusProperty =
			DependencyProperty.Register("CarControllerStatus", typeof(CarControllerStatus), typeof(DeviceStatusUserControl), new PropertyMetadata(default(CarControllerStatus), CarControllerStatusPropertyChanged));

		private static void CarControllerStatusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DeviceStatusUserControl deviceStatusUserControl = (DeviceStatusUserControl) d;
			CarControllerStatus carControllerStatus = (CarControllerStatus) e.NewValue;

			deviceStatusUserControl.UpdateCarControllerStatus(carControllerStatus);
		}

		private void UpdateCarControllerStatus(CarControllerStatus carControllerStatus)
		{
			switch (carControllerStatus)
			{
				case CarControllerStatus.Undefined:
					CarControllerStatusMessage = "No Car Controller Found or An Unknown Error Occurred.";
					break;
				case CarControllerStatus.Disconnected:
					CarControllerStatusMessage = "No Car Controller found!";
					break;
				case CarControllerStatus.Connected:
					CarControllerStatusMessage = "Connected!";
					break;
				default:
					CarControllerStatusMessage = "We got here for some reason";
					throw new NotImplementedException("CarControllerStatus not implemented!");
			}
		}

		public CarControllerStatus CarControllerStatus
		{
			get { return (CarControllerStatus)GetValue(CarControllerStatusProperty); }
			set { SetValue(CarControllerStatusProperty, value); }
		}

		#endregion

		#region KinectStatus
		public static readonly DependencyProperty KinectStatusProperty =
			DependencyProperty.Register("KinectStatus", typeof(KinectStatus), typeof(DeviceStatusUserControl), new PropertyMetadata(default(KinectStatus), KinectStatusPropertyChanged));

		private static void KinectStatusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DeviceStatusUserControl deviceStatusUserControl = (DeviceStatusUserControl) d;
			KinectStatus kinectStatus = (KinectStatus) e.NewValue;

			deviceStatusUserControl.UpdateKinectStatus(kinectStatus);
		}

		private bool _appConflict = false;

		private void UpdateKinectStatus(KinectStatus kinectStatus)
		{
			switch (kinectStatus)
			{
				case KinectStatus.Connected:
					KinectStatusMessage = _appConflict ? "The Kinect is being used by another application." : "Connected!";
					break;

				case KinectStatus.DeviceNotGenuine:
					KinectStatusMessage = "This Sensor Is Not Genuine!";
					break;

				case KinectStatus.DeviceNotSupported:
					KinectStatusMessage = "Kinect for Xbox Not Supported.";
					break;

				case KinectStatus.Disconnected:
					KinectStatusMessage = "No Kinect Sensor Found!";
					break;

				case KinectStatus.Initializing:
					KinectStatusMessage = "Initializing...";
					break;

				case KinectStatus.NotReady:
				case KinectStatus.Error:
					KinectStatusMessage = "Kinect Found, But An Error Occurred. App conflict?";
					break;

				case KinectStatus.Undefined:
					KinectStatusMessage = "No Kinect Found or An Unknown Error Occurred.";
					break;

				case KinectStatus.InsufficientBandwidth:
					KinectStatusMessage = "Too many USB devices. Please unplug one or more!";
					break;

				case KinectStatus.NotPowered:
					KinectStatusMessage = "Kinect power cord is not connected.";
					break;
				default:
					throw new NotImplementedException("KinectStatus not implemented!");
			}
		}

		public KinectStatus KinectStatus
		{
			get { return (KinectStatus)GetValue(KinectStatusProperty); }
			set { SetValue(KinectStatusProperty, value); }
		} 
		#endregion

		#region CarControllerStatusMessage
		private static readonly DependencyProperty CarControllerStatusMessageProperty =
			DependencyProperty.Register("CarControllerStatusMessage", typeof(string), typeof(DeviceStatusUserControl), new PropertyMetadata(default(string)));

		private string CarControllerStatusMessage
		{
			get { return (string)GetValue(CarControllerStatusMessageProperty); }
			set { SetValue(CarControllerStatusMessageProperty, value); }
		} 
		#endregion

		#region KinectStatusMessage
		private static readonly DependencyProperty KinectStatusMessageProperty =
			DependencyProperty.Register("KinectStatusMessage", typeof(string), typeof(DeviceStatusUserControl), new PropertyMetadata(default(string)));

		private string KinectStatusMessage
		{
			get { return (string)GetValue(KinectStatusMessageProperty); }
			set { SetValue(KinectStatusMessageProperty, value); }
		} 
		#endregion

		public static readonly DependencyProperty RetryCommandProperty =
			DependencyProperty.Register("RetryCommand", typeof (ICommand), typeof (DeviceStatusUserControl), new PropertyMetadata(default(ICommand)));

		public ICommand RetryCommand
		{
			get { return (ICommand) GetValue(RetryCommandProperty); }
			set { SetValue(RetryCommandProperty, value); }
		}

		public static readonly DependencyProperty CancelCommandProperty =
			DependencyProperty.Register("CancelCommand", typeof (ICommand), typeof (DeviceStatusUserControl), new PropertyMetadata(default(ICommand)));

		public ICommand CancelCommand
		{
			get { return (ICommand) GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}
	}
	
	[ValueConversion(typeof(CarControllerStatus), typeof(Uri))]
	public class CarControllerStatusToImageSourceUriConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			CarControllerStatus carControllerStatus = (CarControllerStatus)value;
			
			switch (carControllerStatus)
			{
				case CarControllerStatus.Undefined:
				case CarControllerStatus.Disconnected:
					return new Uri(@".\Images\Error.png", UriKind.Relative);
				case CarControllerStatus.Connected:
					return new Uri(@".\Images\Good.png", UriKind.Relative);
				default:
					throw new NotImplementedException("CarControllerStatus enum conversion has not been implemented.");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(KinectStatus), typeof(Uri))]
	public class KinectStatusToImageSourceUriConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			KinectStatus kinectStatus = (KinectStatus) value;

			switch (kinectStatus)
			{
				case KinectStatus.Connected:
					return new Uri(@".\Images\Good.png", UriKind.Relative);
				
				case KinectStatus.DeviceNotSupported:
				case KinectStatus.DeviceNotGenuine:
				case KinectStatus.Disconnected:
				case KinectStatus.NotReady:
				case KinectStatus.Error:
				case KinectStatus.Undefined:
				case KinectStatus.NotPowered:
					return new Uri(@".\Images\Error.png", UriKind.Relative);
			
				case KinectStatus.Initializing:
					return new Uri(@".\Images\Busy.gif", UriKind.Relative);
					
				case KinectStatus.InsufficientBandwidth:
					return new Uri(@".\Images\Warning.png", UriKind.Relative);
				
				default:
					throw new NotImplementedException("KinectStatus enum conversion has not been implemented.");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}


