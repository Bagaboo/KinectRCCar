using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectRCCar.WpfUI
{
	public interface IWpfUISettings
	{
		int MaxRadians { get; }
		int MinRadians { get; }
		double MaxVelocityRange { get; }
		double MinVelocityRange { get; }
		double HorizontalHandThreshold { get; }
		double ZHandThreshold { get; }
		double DirectionDeltaThreshold { get; }
		double VelocityDeltaThreshold { get; }
		int CountdownTime { get; }
		int CountdownInterval { get; }
		double DistanceAboveSpineY { get; }
	}
}
