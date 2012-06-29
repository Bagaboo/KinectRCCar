using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using KinectRCCar.Kinect.Adaptors.Interfaces;
using Microsoft.Kinect;
using Microsoft.Practices.Unity;

namespace KinectRCCar.Kinect.Adaptors
{
	public class KinectSensorAdapter : IKinectSensorAdapter, IDisposable
	{
		private readonly IUnityContainer _container;
		private KinectSensor _kinectSensor;
		private ISkeletonStreamAdapter _skeletonStreamAdapter;
		private IColorImageStreamAdapter _colorStreamAdapter;
		
		public KinectSensorAdapter(IUnityContainer container, KinectSensor kinectSensor)
		{
			_container = container;

			_kinectSensor = kinectSensor;
			KinectSensor.KinectSensors.StatusChanged += KinectSensorsOnStatusChanged;

			WireStreams();
		}

		private void KinectSensorsOnStatusChanged(object sender, StatusChangedEventArgs statusChangedEventArgs)
		{
			if (statusChangedEventArgs.Sensor.UniqueKinectId == null || statusChangedEventArgs.Sensor.UniqueKinectId != _kinectSensor.UniqueKinectId) 
				return;

			WireStreams();
			KinectSensor.KinectSensors.StatusChanged -= KinectSensorsOnStatusChanged;
		}
		
		private void WireStreams()
		{
			if (_kinectSensor.SkeletonStream != null)
			{
				_skeletonStreamAdapter =
					_container.Resolve<ISkeletonStreamAdapter>(new ParameterOverride("skeletonStream", _kinectSensor.SkeletonStream));
			}

			if (_kinectSensor.ColorStream != null)
			{
				_colorStreamAdapter =
					_container.Resolve<IColorImageStreamAdapter>(new ParameterOverride("colorImageStream", _kinectSensor.ColorStream));
			}
		}

		public bool IsRunning
		{
			get { return _kinectSensor.IsRunning; }
		}

		public ISkeletonStreamAdapter SkeletonStream
		{
			get { return _skeletonStreamAdapter; }
		}

		public KinectStatus Status
		{
			get { return _kinectSensor.Status; }
		}

		public IColorImageStreamAdapter ColorStream
		{
			get { return _colorStreamAdapter; }
		}

		public int ElevationAngle
		{
			get { return _kinectSensor.ElevationAngle; }
			set { _kinectSensor.ElevationAngle = value; }
		}

		public int MaxElevationAngle
		{
			get { return _kinectSensor.MaxElevationAngle; }
		}

		public int MinElevationAngle
		{
			get { return _kinectSensor.MinElevationAngle; }
		}

		public string UniqueKinectId
		{
			get { return _kinectSensor.UniqueKinectId; }
		}

		public void Stop()
		{
			_kinectSensor.Stop();
		}

		public void Start()
		{
			_kinectSensor.Start();
		}

		public event EventHandler<ColorImageFrameReadyEventArgs> ColorFrameReady
		{
			add { _kinectSensor.ColorFrameReady += value; }
			remove { _kinectSensor.ColorFrameReady -= value; }
		}

		public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady
		{
			add { _kinectSensor.SkeletonFrameReady += value; }
			remove { _kinectSensor.SkeletonFrameReady -= value; }
		}

		private bool _disposed;

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					KinectSensor.KinectSensors.StatusChanged -= KinectSensorsOnStatusChanged;
					_skeletonStreamAdapter = null;
					_colorStreamAdapter = null;
					_kinectSensor = null;
				}

				_disposed = true;
			}
		}
	}
}
