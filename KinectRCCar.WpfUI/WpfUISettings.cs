using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRCCar.WpfUI.Properties;

namespace KinectRCCar.WpfUI
{
	internal class WpfUISettings : IWpfUISettings
	{
		private readonly Settings _settings = Settings.Default;

		public int MaxRadians
		{
			get { return _settings.MaxRadians; }
		}

		public int MinRadians
		{
			get { return _settings.MinRadians; }
		}

		public double MaxVelocityRange
		{
			get { return _settings.MaxVelocityRange; }
		}

		public double MinVelocityRange
		{
			get { return _settings.MinVelocityRange; }
		}

		public double HorizontalHandThreshold
		{
			get { return _settings.HorizontalHandThreshold; }
		}

		public double ZHandThreshold
		{
			get { return _settings.ZHandThreshold; }
		}

		public double DirectionDeltaThreshold
		{
			get { return _settings.DirectionDeltaThreshold; }
		}

		public double VelocityDeltaThreshold
		{
			get { return _settings.VelocityDeltaThreshold; }
		}

		public int CountdownTime
		{
			get { return _settings.CountdownTime; }
		}

		public int CountdownInterval
		{
			get { return _settings.CountdownInterval; }
		}

		public double DistanceAboveSpineY
		{
			get { return _settings.DistanceAboveSpineY; }
		}
	}
}
