using System;
using System.Windows;
using System.Windows.Controls;

namespace KinectRCCar.UserControls
{
	/// <summary>
	/// Interaction logic for CameraSettingsUserControl.xaml
	/// </summary>
	public partial class CameraSettingsUserControl : UserControl
	{
		public CameraSettingsUserControl()
		{
			InitializeComponent();
		}

		#region Address
		public static readonly DependencyProperty AddressProperty =
			DependencyProperty.Register("Address", typeof(Uri), typeof(CameraSettingsUserControl), new FrameworkPropertyMetadata(default(Uri), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string Address
		{
			get { return (string)GetValue(AddressProperty); }
			set { SetValue(AddressProperty, value); }
		} 
		#endregion

		#region Username
		public static readonly DependencyProperty UsernameProperty =
			DependencyProperty.Register("Username", typeof(string), typeof(CameraSettingsUserControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string Username
		{
			get { return (string)GetValue(UsernameProperty); }
			set { SetValue(UsernameProperty, value); }
		} 
		#endregion

		#region Password
		public static readonly DependencyProperty PasswordProperty =
			DependencyProperty.Register("Password", typeof(string), typeof(CameraSettingsUserControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string Password
		{
			get { return (string)GetValue(PasswordProperty); }
			set { SetValue(PasswordProperty, value); }
		} 
		#endregion

	}
}
