using Microsoft.Kinect;

namespace KinectRCCar.Kinect.Adaptors.Interfaces
{
	public interface IColorImageStreamAdapter
	{
		void Enable(ColorImageFormat colorImageFormat);
	}
}
