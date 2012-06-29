using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectRCCar.Kinect
{
	public interface IKinectSettings
	{
		float Smoothing { get; }
		float Correction { get; }
		float Prediction { get; }
		float JitterRadius { get; }
		float MaxDeviationRadius { get; }
		bool SmoothingEnabled { get; }
	}
}
