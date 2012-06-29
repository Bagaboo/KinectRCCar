using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace KinectRCCar.Infrastructure.Interfaces
{
	/// <summary>
	/// <para>Inherit from this interface to roll your own Camera Service that is used by the RC Car.</para>
	/// </summary>
	public interface ICarCameraService
	{
		#region Methods

		/// <summary>
		/// <para>Starts retrieving images from the RC Car camera.</para>
		/// </summary>
		void Start();
		/// <summary>
		/// <para>Stops retrieving images from the RC Car camera.</para>
		/// </summary>
		void Stop(); 
		
		#endregion Methods
		
		#region Properties
		
		/// <summary>
		/// <para>The absolute uri address to the video source of the RC Car camera.</para>
		/// </summary>
		Uri CameraAddress { get; set; }
		/// <summary>
		/// <para>The username (if required) to access the RC Car Camera.</para>
		/// </summary>
		string Username { get; set; }
		/// <summary>
		/// <para>The password (if required) to access the RC Car camera.</para>
		/// </summary>
		string Password { get; set; }

		/// <summary>
		/// <para>Indicates if the Camera Service is currently receiving images from the camera on the RC Car.</para>
		/// </summary>
		bool IsRunning { get; }

		#endregion

		#region Events
		
		/// <summary>
		/// <para>Fires when an image has been retrieved from the RC Car camera.</para>
		/// </summary>
		 
		event EventHandler<ImageReadyEventArgs> ImageReady;

		#endregion Events
	}

	/// <summary>
	/// <para>Returns the image retrieved from the RC Car camera. </para>
	/// </summary>
	public class ImageReadyEventArgs : EventArgs
	{
		/// <summary>
		/// <para>The image retrieved from the RC Car camera.</para>
		/// </summary>
		public BitmapImage BitmapImage { get; set; }
	}
}