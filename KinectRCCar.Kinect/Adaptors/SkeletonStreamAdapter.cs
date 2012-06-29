using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRCCar.Kinect.Adaptors.Interfaces;
using Microsoft.Kinect;

namespace KinectRCCar.Kinect.Adaptors
{
	class SkeletonStreamAdapter : ISkeletonStreamAdapter
	{
		private readonly SkeletonStream _skeletonStream;

		public SkeletonStreamAdapter(SkeletonStream skeletonStream)
		{
			_skeletonStream = skeletonStream;
		}

		public void Enable()
		{
			_skeletonStream.Enable();
		}

		public void Enable(TransformSmoothParameters transformSmoothParameters)
		{
			_skeletonStream.Enable(transformSmoothParameters);
		}
	}
}
