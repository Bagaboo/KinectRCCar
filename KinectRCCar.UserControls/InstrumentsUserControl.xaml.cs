using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KinectRCCar.UserControls
{
	/// <summary>
	/// Interaction logic for InstrumentsUserControl.xaml
	/// </summary>
	public partial class InstrumentsUserControl : UserControl
	{
		public InstrumentsUserControl()
		{
			InitializeComponent();
			if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
			{
				CountDown = new TimeSpan(0,0,0,3);
			}

		}

		#region MaximumKinectAngle
		public static readonly DependencyProperty MaximumKinectAngleProperty =
			DependencyProperty.Register("MaximumKinectAngle", typeof(int), typeof(InstrumentsUserControl), new PropertyMetadata(27));

		public int MaximumKinectAngle
		{
			get { return (int)GetValue(MaximumKinectAngleProperty); }
			set { SetValue(MaximumKinectAngleProperty, value); }
		} 
		#endregion

		#region MinimumKinectAngle
		public static readonly DependencyProperty MinimumKinectAngleProperty =
			DependencyProperty.Register("MinimumKinectAngle", typeof(int), typeof(InstrumentsUserControl), new PropertyMetadata(-27));

		public int MinimumKinectAngle
		{
			get { return (int)GetValue(MinimumKinectAngleProperty); }
			set { SetValue(MinimumKinectAngleProperty, value); }
		} 
		#endregion



		#region KinectAngle

		public static readonly DependencyProperty KinectAngleProperty =
			DependencyProperty.Register("KinectAngle", typeof (int), typeof (InstrumentsUserControl), new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, KinectAnglePropertyChanged));

		private static void KinectAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			InstrumentsUserControl instrumentsUserControl = (InstrumentsUserControl)d;
			instrumentsUserControl.HasKinectAngleChanged = true;
		}

		public int KinectAngle
		{
			get { return (int) GetValue(KinectAngleProperty); }
			set { SetValue(KinectAngleProperty, value); }
		}
		#endregion

		#region HasKinectAngleChanged
		private static readonly DependencyProperty HasKinectAngleChangedProperty =
			DependencyProperty.Register("HasKinectAngleChanged", typeof(bool), typeof(InstrumentsUserControl), new PropertyMetadata(false));

		private bool HasKinectAngleChanged
		{
			get { return (bool)GetValue(HasKinectAngleChangedProperty); }
			set { SetValue(HasKinectAngleChangedProperty, value); }
		} 
		#endregion

		#region SetAngleCommand
		public static readonly DependencyProperty SetAngleCommandProperty =
			DependencyProperty.Register("SetAngleCommand", typeof(ICommand), typeof(InstrumentsUserControl), new PropertyMetadata(default(ICommand)));

		public ICommand SetAngleCommand
		{
			get { return (ICommand)GetValue(SetAngleCommandProperty); }
			set { SetValue(SetAngleCommandProperty, value); }
		} 
		#endregion

		#region Velocity
		public static readonly DependencyProperty VelocityProperty =
			DependencyProperty.Register("Velocity", typeof(double), typeof(InstrumentsUserControl), new PropertyMetadata(0.0));

		public double Velocity
		{
			get { return (double)GetValue(VelocityProperty); }
			set { SetValue(VelocityProperty, value); }
		} 
		#endregion

		#region Direction
		public static readonly DependencyProperty DirectionProperty =
			DependencyProperty.Register("Direction", typeof(double), typeof(InstrumentsUserControl), new PropertyMetadata(0.0));

		public double Direction
		{
			get { return (double)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		} 
		#endregion

		#region CountDown

		public static readonly DependencyProperty CountDownProperty =
			DependencyProperty.Register("CountDown", typeof (TimeSpan), typeof (InstrumentsUserControl), new PropertyMetadata(default(TimeSpan)));

		public TimeSpan CountDown
		{
			get { return (TimeSpan) GetValue(CountDownProperty); }
			set { SetValue(CountDownProperty, value); }
		}
		#endregion

		private void SetAngleButtonClick(object sender, RoutedEventArgs e)
		{
			HasKinectAngleChanged = false;
		}


	}
}
