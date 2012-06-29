using Microsoft.Kinect;

namespace KinectRCCar.Kinect.Adaptors.Interfaces
{
	public interface ISkeletonStreamAdapter
	{
		void Enable();
		void Enable(TransformSmoothParameters transformSmoothParameters);
	}
}
