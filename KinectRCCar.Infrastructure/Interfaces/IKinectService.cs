using System;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace KinectRCCar.Infrastructure.Interfaces
{
	public interface IKinectService
	{
		event EventHandler<SkeletonArrayReadyEventArgs> SkeletonFrameReady;
		event EventHandler<ColorImageReadyEventArgs> ColorImageReady;
		event EventHandler<KinectStatusEventArgs> KinectStatusChanged;
		
		void Start();
		void Stop();

		int ElevationAngle { get; set; }
		int MaximumElevation { get; }
		int MinimumElevation { get; }

		KinectStatus Status { get; }
		bool IsRunning { get; }
	}

	public class ColorImageReadyEventArgs : EventArgs
	{
		public BitmapSource BitmapSource { get; set; }
	}

	public class SkeletonArrayReadyEventArgs : EventArgs
	{
		public Skeleton[] Skeletons { get; set; }
	}

	public class KinectStatusEventArgs : EventArgs
	{
		public KinectStatus Status { get; set; }
	}
}
