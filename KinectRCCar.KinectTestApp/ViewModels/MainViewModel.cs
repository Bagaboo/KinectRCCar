using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using KinectRCCar.Kinect;
using Microsoft.Kinect;
using System.Windows;

namespace KinectRCCar.KinectTestApp.ViewModels
{
	public class MainViewModel : ViewModelBase, IDisposable
	{
		private readonly IKinectService _kinectService;

		public MainViewModel()
		{
			_kinectService = new KinectService();


			_kinectService.SkeletonFrameReady += _kinectService_SkeletonFrameReady;
			_kinectService.Initialize();
		}

		void _kinectService_SkeletonFrameReady(object sender, SkeletonArrayReadyEventArgs e)
		{
			Skeletons = e.SkeletonArray;

			Skeleton skeleton = Skeletons.GetFirstTrackedSkeleton();

			if (skeleton == null) return;

			Vector3D leftHand = GetJointVector(JointType.HandLeft, skeleton);

			Vector3D rightHand = GetJointVector(JointType.HandRight, skeleton);

			Vector3D handsMidpoint = new Vector3D((rightHand.X + leftHand.X)/2.0, (rightHand.Y + leftHand.Y)/2.0, (rightHand.Z + leftHand.Z)/ 2.0);

			PositionX = skeleton.Position.X;
			PositionY = skeleton.Position.Y;
			PositionZ = skeleton.Position.Z;

			Velocity = (skeleton.Position.Z - handsMidpoint.Z)/0.1;

		}

		private static Vector3D GetJointVector(JointType joint, Skeleton skeleton)
		{
			Vector3D newVector = new Vector3D
			{
				X = skeleton.Joints[joint].Position.X,
				Y = skeleton.Joints[joint].Position.Y,
				Z = skeleton.Joints[joint].Position.Z
			};

			return newVector;
		}
		
		private double _positionX;
		public double PositionX
		{
			get { return _positionX; }
			set
			{
				_positionX = value;
				RaisePropertyChanged("PositionX");
			}
		}

		private double _positionY;
		public double PositionY
		{
			get { return _positionY; }
			set
			{
				_positionY = value;
				RaisePropertyChanged("PositionY");
			}
		}

		private double _positionZ;
		public double PositionZ
		{
			get { return _positionZ; }
			set
			{
				_positionZ = value;
				RaisePropertyChanged("PositionZ");
			}
		}

		private double _velocity;
		public double Velocity
		{
			get { return _velocity; }
			set
			{
				_velocity = value;
				RaisePropertyChanged("Velocity");
			}
		}

		private Skeleton[] _skeletons;
		public Skeleton[] Skeletons
		{
			get { return _skeletons; }
			set
			{
				_skeletons = value;
				RaisePropertyChanged("Skeleton");
			}
		}

		public void Dispose()
		{
			_kinectService.Uninitialize();
		}
	}
}
