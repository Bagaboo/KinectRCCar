using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectRCCar.CarController
{
	public interface ICarControllerSettings
	{
		string PortSearchKey { get; }
		int DirectionRange { get; }
		int DirectionMidpoint { get; }
		int ForwardMaximum { get; }
		int ForwardMinimum { get; }
		int ReverseMaximum { get; }
		int ReverseMinimum { get; }
	}
}
