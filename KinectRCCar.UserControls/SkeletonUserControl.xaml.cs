using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;

namespace KinectRCCar.UserControls
{
	/// <summary>
	/// Interaction logic for SkeletonUserControl.xaml
	/// </summary>
	public partial class SkeletonUserControl : UserControl
	{
		public SkeletonUserControl()
		{
			InitializeComponent();
		}

		static readonly Dictionary<JointType, Brush> JointColors = new Dictionary<JointType, Brush>() { 
			{JointType.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
			{JointType.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
			{JointType.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
			{JointType.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
			{JointType.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
			{JointType.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
			{JointType.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
			{JointType.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
			{JointType.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
			{JointType.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
			{JointType.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
			{JointType.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
			{JointType.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
			{JointType.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
			{JointType.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
			{JointType.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
			{JointType.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
			{JointType.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
			{JointType.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
			{JointType.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
		};

		public Skeleton[] Skeletons
		{
			get { return (Skeleton[])GetValue(SkeletonsProperty); }
			set { SetValue(SkeletonsProperty, value); }
		}

		public static readonly DependencyProperty SkeletonsProperty =
			DependencyProperty.Register("Skeletons", typeof(Skeleton[]), typeof(SkeletonUserControl), new PropertyMetadata(default(Skeleton[]), SkeletonsPropertyChanged));

		private static void SkeletonsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SkeletonUserControl suc = (SkeletonUserControl) d;
			Skeleton[] skeletons = (Skeleton[])e.NewValue;
			suc.SkeletonCanvas.Children.Clear();
			
			if (skeletons == null)
				return;

			Skeleton controllingSkeleton = (from s in skeletons
											 where s.TrackingState == SkeletonTrackingState.Tracked
											 orderby s.Position.Z
											 select s).FirstOrDefault();
			
			if (controllingSkeleton == null) return;

			int iSkeleton = 0;
			Brush[] brushes = new Brush[6];
			brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
			brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
			brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
			brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
			brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
			brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

			

			if (SkeletonTrackingState.Tracked != controllingSkeleton.TrackingState) return;
			
			// Draw bones
			Brush brush = brushes[iSkeleton % brushes.Length];
			suc.SkeletonCanvas.Children.Add(suc.GetBodySegment(controllingSkeleton.Joints, brush, JointType.HipCenter, JointType.Spine, JointType.ShoulderCenter, JointType.Head));
			suc.SkeletonCanvas.Children.Add(suc.GetBodySegment(controllingSkeleton.Joints, brush, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft));
			suc.SkeletonCanvas.Children.Add(suc.GetBodySegment(controllingSkeleton.Joints, brush, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight));
			suc.SkeletonCanvas.Children.Add(suc.GetBodySegment(controllingSkeleton.Joints, brush, JointType.HipCenter, JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft, JointType.FootLeft));
			suc.SkeletonCanvas.Children.Add(suc.GetBodySegment(controllingSkeleton.Joints, brush, JointType.HipCenter, JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight));

			// Draw joints
			foreach (Joint joint in controllingSkeleton.Joints)
			{
				Point jointPos = suc.SetUiPosition(joint);
				Line jointLine = new Line();
				jointLine.X1 = jointPos.X - 3;
				jointLine.X2 = jointLine.X1 + 6;
				jointLine.Y1 = jointLine.Y2 = jointPos.Y;
				jointLine.Stroke = JointColors[joint.JointType];
				jointLine.StrokeThickness = 6;
				suc.SkeletonCanvas.Children.Add(jointLine);
			}
		}

		private Polyline GetBodySegment(JointCollection joints, Brush brush, params JointType[] ids)
		{
			PointCollection points = new PointCollection(ids.Length);
			foreach (JointType t in ids)
			{
				points.Add(SetUiPosition(joints[t]));
			}

			Polyline polyline = new Polyline();
			polyline.Points = points;
			polyline.Stroke = brush;
			polyline.StrokeThickness = 5;
			return polyline;
		}

		private Point SetUiPosition(Joint joint)
		{
			if (ActualWidth > 0 && ActualHeight > 0)
			{
				Joint scaledJoint = joint.ScaleTo((int)ActualWidth, (int)ActualHeight, 1f, 1f);
				return new Point
				{
					X = scaledJoint.Position.X, // - ui.ActualWidth / 2,
					Y = scaledJoint.Position.Y // - ui.ActualHeight / 2
				};
			}

			return new Point();
		}
	}
}
