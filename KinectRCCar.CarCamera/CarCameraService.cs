using System;
using KinectRCCar.Infrastructure.Interfaces;
using KinectRCCar.MjpegProcessor;

namespace KinectRCCar.CarCamera
{
	public class CarCameraService : ICarCameraService
	{
		private readonly MjpegDecoder _mjpegDecoder;

		public CarCameraService(MjpegDecoder mjpegDecoder)
		{
			_mjpegDecoder = mjpegDecoder;
		}

		public void Start()
		{
			_mjpegDecoder.FrameReady += MjpegDecoderFrameReady;


			if (CameraAddress != null && !CameraAddress.IsWellFormedOriginalString()) 
				return;


			try
			{
				_mjpegDecoder.ParseStream(CameraAddress, Username, Password);
				IsRunning = true;
			}
			catch (Exception)
			{
				IsRunning = false;
				Stop();
			}
		}

		public Uri CameraAddress { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public void Stop()
		{
			_mjpegDecoder.StopStream();
			_mjpegDecoder.FrameReady -= MjpegDecoderFrameReady;
			IsRunning = true;
		}

		public bool IsRunning { get; private set; }

		public event EventHandler<ImageReadyEventArgs> ImageReady;

		private void OnImageReady(FrameReadyEventArgs e)
		{
			EventHandler<ImageReadyEventArgs> handler = ImageReady;
			if (handler != null) handler(this, new ImageReadyEventArgs() { BitmapImage = e.BitmapImage});
		}

		private void MjpegDecoderFrameReady(object sender, FrameReadyEventArgs e)
		{
			OnImageReady(e);
		}
	}
}
