using KinectRCCar.Kinect.Adaptors.Interfaces;
using Microsoft.Kinect;

namespace KinectRCCar.Kinect.Adaptors
{
	class ColorImageStreamAdapter : IColorImageStreamAdapter
	{
		private readonly ColorImageStream _colorImageStream;

		public ColorImageStreamAdapter(ColorImageStream colorImageStream)
		{
			_colorImageStream = colorImageStream;
		}

		public void Enable(ColorImageFormat colorImageFormat)
		{
			_colorImageStream.Enable(colorImageFormat);
		}
	}
}
