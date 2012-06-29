using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRCCar.Kinect.Properties;

namespace KinectRCCar.Kinect
{
	public class KinectSettings : IKinectSettings
	{
		private readonly Settings _settings = Settings.Default;

		public float Smoothing
		{
			get { return _settings.Smoothing; }
		}

		public float Correction
		{
			get { return _settings.Correction; }
		}

		public float Prediction
		{
			get { return _settings.Prediction; }
		}

		public float JitterRadius
		{
			get { return _settings.JitterRadius; }
		}

		public float MaxDeviationRadius
		{
			get { return _settings.MaxDeviationRadius; }
		}

		public bool SmoothingEnabled
		{
			get { return _settings.SmoothingEnabled; }
		}
	}
}
