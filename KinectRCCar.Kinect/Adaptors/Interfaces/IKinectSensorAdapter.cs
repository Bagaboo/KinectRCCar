using System;
using Microsoft.Kinect;

namespace KinectRCCar.Kinect.Adaptors.Interfaces
{
	public interface IKinectSensorAdapter : IDisposable
	{
		bool IsRunning { get; }
		ISkeletonStreamAdapter SkeletonStream { get; }
		KinectStatus Status { get;  }
		IColorImageStreamAdapter ColorStream { get; }
		int ElevationAngle { get; set; }
		int MaxElevationAngle { get; }
		int MinElevationAngle { get; }
		string UniqueKinectId { get; }

		void Stop();
		void Start();

		event EventHandler<ColorImageFrameReadyEventArgs> ColorFrameReady;
		event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;
	}
}
