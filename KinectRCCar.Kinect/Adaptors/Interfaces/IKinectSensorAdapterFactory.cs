using System;
using Microsoft.Kinect;

namespace KinectRCCar.Kinect.Adaptors.Interfaces
{
	public interface IKinectSensorAdapterFactory
	{
		IKinectSensorAdapter GetFirstKinectSensor();
		event EventHandler<StatusChangedEventArgs> StatusChanged;
	}
}
