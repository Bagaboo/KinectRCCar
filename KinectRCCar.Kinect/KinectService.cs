using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coding4Fun.Kinect.Wpf;
using KinectRCCar.Infrastructure.Interfaces;
using KinectRCCar.Kinect.Adaptors;
using KinectRCCar.Kinect.Adaptors.Interfaces;
using KinectRCCar.Kinect.Properties;
using Microsoft.Kinect;
using Microsoft.Practices.Unity;
using System.Windows.Threading;


namespace KinectRCCar.Kinect
{
	public class KinectService : IKinectService, IDisposable
	{
		private readonly IKinectSensorAdapterFactory _kinectSensorAdapterFactory;

		private readonly IKinectSettings _settings;

		public KinectService(IKinectSensorAdapterFactory kinectSensorAdapterFactory, IKinectSettings settings)
		{
			_kinectSensorAdapterFactory = kinectSensorAdapterFactory;
			_settings = settings;

			_kinectSensorAdapterFactory.StatusChanged += KinectSensorsOnStatusChanged;

			KinectAdapter = _kinectSensorAdapterFactory.GetFirstKinectSensor();

			OnKinectStatusChanged(KinectAdapter != null ? KinectAdapter.Status : KinectStatus.Disconnected);
		}
		
		private void KinectSensorsOnStatusChanged(object sender, StatusChangedEventArgs e)
		{
			if (KinectAdapter == null)
			{
				KinectAdapter = _kinectSensorAdapterFactory.GetFirstKinectSensor();
			}
			else
			{
				if (e.Sensor.UniqueKinectId == KinectAdapter.UniqueKinectId && e.Status == KinectStatus.Disconnected)
				{
					KinectAdapter.Dispose();
					KinectAdapter = null;
				}
			}

			OnKinectStatusChanged(e.Status);
		}

		public KinectStatus Status { get; private set; }

		public void Start()
		{
			if (KinectAdapter == null || KinectAdapter.Status != KinectStatus.Connected)
				return;

			KinectAdapter.ColorFrameReady += KinectOnColorFrameReady;
			KinectAdapter.SkeletonFrameReady += KinectOnSkeletonFrameReady;

			KinectAdapter.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

			if (_settings.SmoothingEnabled)
			{
				KinectAdapter.SkeletonStream.Enable(new TransformSmoothParameters
					                            {
					                             	Smoothing = _settings.Smoothing,
													Correction = _settings.Correction,
													Prediction = _settings.Prediction,
													JitterRadius = _settings.JitterRadius,
					                             	MaxDeviationRadius = _settings.MaxDeviationRadius
					                            });
			}
			else
			{
				KinectAdapter.SkeletonStream.Enable();
			}

			try
			{
				KinectAdapter.Start();
			}
			catch (Exception)
			{
				Stop();
				OnKinectStatusChanged(KinectStatus.Error);
			}
			
		}

		private Skeleton[] _skeletonArray;

		

		private void KinectOnColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
		{
			if (!IsRunning)
				return;

			#region ColorFrameProcessing
			
			// Save the imageFrame data to a BitmapSource then fire the 
			// ColorImageReady event.
			// Since the BitmapSource is a dependency object it is aware
			// of it's thread affinity and so Freeze() must be called
			// in order to allow the data to be read by the UI thread.
			// The creation of the BitmapSource is taken care of by the
			// extention method ToBitmapSource.
			using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
			{
				if (imageFrame != null)
				{
					BitmapSource image = imageFrame.ToBitmapSource();
					image.Freeze();
					
					OnColorImageReady(image);
				}
			} 
			#endregion
		}

		private void KinectOnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
		{
			if(!IsRunning)
				return;

			using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
			{
				if (skeletonFrame == null) return;

				_skeletonArray = new Skeleton[skeletonFrame.SkeletonArrayLength];

				skeletonFrame.CopySkeletonDataTo(_skeletonArray);

				OnSkeletonFrameReady(_skeletonArray);
			}
		}

		public bool IsRunning
		{
			get { return KinectAdapter != null && KinectAdapter.IsRunning; }
		}

		public void Stop()
		{
			if (KinectAdapter == null || !KinectAdapter.IsRunning) return;

			KinectAdapter.ColorFrameReady -= KinectOnColorFrameReady;
			KinectAdapter.SkeletonFrameReady -= KinectOnSkeletonFrameReady;

			KinectAdapter.Stop();
		}

		private IKinectSensorAdapter KinectAdapter { get; set; }

		#region Properties
		public int ElevationAngle
		{
			get { return KinectAdapter.ElevationAngle; }
			set { KinectAdapter.ElevationAngle = value; }
		}

		public int MaximumElevation
		{
			get { return KinectAdapter.MaxElevationAngle; }
		}

		public int MinimumElevation
		{
			get { return KinectAdapter.MinElevationAngle; }
		} 
		#endregion

		#region Events

		public event EventHandler<SkeletonArrayReadyEventArgs> SkeletonFrameReady;

		public void OnSkeletonFrameReady(Skeleton[] skeletons)
		{
			EventHandler<SkeletonArrayReadyEventArgs> handler = SkeletonFrameReady;
			if (handler != null) handler(this, new SkeletonArrayReadyEventArgs { Skeletons = skeletons});
		}

		public event EventHandler<ColorImageReadyEventArgs> ColorImageReady;

		public void OnColorImageReady(BitmapSource bitmapSource)
		{
			EventHandler<ColorImageReadyEventArgs> handler = ColorImageReady;
			if (handler != null) handler(this, new ColorImageReadyEventArgs { BitmapSource = bitmapSource});
		}

		public event EventHandler<KinectStatusEventArgs> KinectStatusChanged;

		public void OnKinectStatusChanged(KinectStatus status)
		{
			Status = status;
			EventHandler<KinectStatusEventArgs> handler = KinectStatusChanged;
			if (handler != null) handler(this, new KinectStatusEventArgs() { Status = status});
		}

		#endregion

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
					Stop();
					_kinectSensorAdapterFactory.StatusChanged -= KinectSensorsOnStatusChanged;
				}

				_disposed = true;
			}
		}
	}
}
