using System;

namespace KinectRCCar.CarController.Serial
{
	public interface IWmiMbedHelper : IDisposable
	{
		void Start();
		void Stop();
		event EventHandler<MbedAttachedEventArgs> MbedAttachedEvent;
	}

	public class MbedAttachedEventArgs : EventArgs
	{
		public MbedAttachedStatus Status { get; set; }
	}

	public enum MbedAttachedStatus
	{
		Undefined,
		Detached,
		Attached
	}
}