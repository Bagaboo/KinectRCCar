using System.Windows;
using System.Windows.Controls;

namespace KinectRCCar.UserControls
{
	public sealed class BindablePasswordBox : Decorator
	{
		public static readonly DependencyProperty PasswordProperty =
			DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox),new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string Password
		{
			get { return (string)GetValue(PasswordProperty); }
			set { SetValue(PasswordProperty, value); }
		}

		public BindablePasswordBox()
		{
			Child = new PasswordBox();
			((PasswordBox)Child).PasswordChanged += BindablePasswordBox_PasswordChanged;
		}

		void BindablePasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			Password = ((PasswordBox)Child).Password;
		}

	}
}
