using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRCCar.CarController.Properties;

namespace KinectRCCar.CarController
{
	public class CarControllerSettings : ICarControllerSettings
	{
		private readonly Settings _settings = Settings.Default;

		public string PortSearchKey
		{
			get { return _settings.PortSearchKey; }
		}

		public int DirectionRange
		{
			get { return _settings.DirectionRange; }
		}

		public int DirectionMidpoint
		{
			get { return _settings.DirectionMidpoint; }
		}

		public int ForwardMaximum
		{
			get { return _settings.ForwardMaximum; }
		}

		public int ForwardMinimum
		{
			get { return _settings.ForwardMinimum; }
		}

		public int ReverseMaximum
		{
			get { return _settings.ReverseMaximum; }
		}

		public int ReverseMinimum
		{
			get { return _settings.ReverseMinimum; }
		}
	}
}
