using System;

namespace KinectRCCar.CarController.Serial
{
	public interface IMbedService : IDisposable
	{
		void Start();
		void Stop();

		string RPC(string function, params int[] arguments);

		bool IsRunning { get; }
		
		MbedStatus Status { get; }

		event EventHandler<MbedStatusEventArgs> MbedStatusChanged;

	}

	public class MbedStatusEventArgs : EventArgs
	{
		public MbedStatus Status { get; set; }
	}

	public enum MbedStatus
	{
		Undefined,
		Disconnected,
		Connected,
		NoDriver,
		Error
	}
}
