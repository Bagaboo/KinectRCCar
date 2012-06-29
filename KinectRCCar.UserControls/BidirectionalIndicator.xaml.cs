using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KinectRCCar.UserControls
{
	/// <summary>
	/// Interaction logic for BidirectionalIndicator.xaml
	/// </summary>
	public partial class BidirectionalIndicator : UserControl
	{
		public BidirectionalIndicator()
		{
			InitializeComponent();
		}

		#region Value
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(double), typeof(BidirectionalIndicator), new PropertyMetadata(0.0, ValuePropertyChanged));

		private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BidirectionalIndicator bidirectionalIndicator = (BidirectionalIndicator)d;
			double value = (double)e.NewValue;

			if (value > 0)
			{
				bidirectionalIndicator.negativeProgressBar.Value = 0.0;
				bidirectionalIndicator.positiveProgressBar.Value = value;
			}
			else if (value < 0)
			{
				bidirectionalIndicator.negativeProgressBar.Value = -value;
				bidirectionalIndicator.positiveProgressBar.Value = 0.0;
			}
			else
			{
				bidirectionalIndicator.negativeProgressBar.Value = 0.0;
				bidirectionalIndicator.positiveProgressBar.Value = 0.0;
			}
		}

		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		#endregion

		#region Maximum
		public static readonly DependencyProperty MaximumProperty =
			DependencyProperty.Register("Maximum", typeof(double), typeof(BidirectionalIndicator), new PropertyMetadata(10.0));

		public double Maximum
		{
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}
		#endregion

		#region Minimum
		public static readonly DependencyProperty MinimumProperty =
			DependencyProperty.Register("Minimum", typeof(double), typeof(BidirectionalIndicator), new PropertyMetadata(-10.0));

		public double Minimum
		{
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}
		#endregion

		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(BidirectionalIndicator), new PropertyMetadata(Orientation.Horizontal, OrientationPropertyChanged));

		private static void OrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BidirectionalIndicator bidirectionalProgressbar = (BidirectionalIndicator)d;
			Orientation orientation = (Orientation)e.NewValue;

			// Don't do anything if the orientation is the same as before.
			if (orientation == (Orientation)e.OldValue)
				return;

			switch (orientation)
			{
				case Orientation.Horizontal:
					// convert the layout grid from 1x2 to 2x1
					bidirectionalProgressbar.layoutRoot.RowDefinitions.RemoveAt(1);
					bidirectionalProgressbar.layoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

					// reposition the progressbars to their appropriate cells
					Grid.SetColumn(bidirectionalProgressbar.positiveProgressBar, 1);
					Grid.SetRow(bidirectionalProgressbar.negativeProgressBar, 0);
					
					// switch the progressbars' orientations
					bidirectionalProgressbar.negativeProgressBar.Orientation = Orientation.Horizontal;
					bidirectionalProgressbar.positiveProgressBar.Orientation = Orientation.Horizontal;
				
					// transform the negativeProgressBar by adding a rotation of 180 degrees
					// scoping brackets used to avoid transformGroup name conflict
					{
						TransformGroup transformGroup = (TransformGroup)bidirectionalProgressbar.negativeProgressBar.RenderTransform;
						transformGroup.Children.Add(new RotateTransform(180));
					}
					break;

				case Orientation.Vertical:
					// convert the layout grid from 2x1 to 1x2
					bidirectionalProgressbar.layoutRoot.ColumnDefinitions.RemoveAt(1);
					bidirectionalProgressbar.layoutRoot.RowDefinitions.Add(new RowDefinition());

					// reposition progressbars to their appropriate cells
					Grid.SetRow(bidirectionalProgressbar.negativeProgressBar, 1);
					Grid.SetColumn(bidirectionalProgressbar.positiveProgressBar, 0);

					// switch the progressbars' orientations
					bidirectionalProgressbar.negativeProgressBar.Orientation = Orientation.Vertical;
					bidirectionalProgressbar.positiveProgressBar.Orientation = Orientation.Vertical;
					
					// transform the negativeprogressbar by removing the 180 degree rotation
					// scoping brackets used to avoid transformGroup name conflict
					{
						TransformGroup transformGroup = (TransformGroup)bidirectionalProgressbar.negativeProgressBar.RenderTransform;
						transformGroup.Children.RemoveAt(1);
					}
					break;

				default:
					throw new NotImplementedException("Orientation not implemented.");
			}
		}

		public Orientation Orientation
		{
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
	}
}
