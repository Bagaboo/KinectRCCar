using System.Linq;
using Microsoft.Kinect;

namespace KinectRCCar.Kinect
{
	public static class RuntimeExtensions
	{
		//public static void SetSmoothTransform(this Runtime runtime)
		//{
		//    runtime.SkeletonEngine.TransformSmooth = true;

		//    var parameters = new TransformSmoothParameters
		//    {
		//        Smoothing = Settings.Default.Smoothing,
		//        Correction = Settings.Default.Correction,
		//        Prediction = Settings.Default.Prediction,
		//        JitterRadius = Settings.Default.JitterRadius,
		//        MaxDeviationRadius = Settings.Default.MaxDeviationRadius
		//    };

		//    runtime.SkeletonEngine.SmoothParameters = parameters;
		//}

		public static Skeleton GetFirstTrackedSkeleton(this Skeleton[] skeletons)
		{
			Skeleton data = (from s in skeletons
							where s.TrackingState == SkeletonTrackingState.Tracked
							orderby s.Position.Z
							select s).FirstOrDefault();
			return data;
		}
	}
}
