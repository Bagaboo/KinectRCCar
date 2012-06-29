using System;

namespace KinectRCCar.WpfUI
{
	public interface IControlLogic
	{
		event EventHandler<NewDirectionEventArgs> NewDirectionEvent;
		event EventHandler<NewVelocityEventArgs> NewVelocityEvent;
		event EventHandler<CountdownTickEventArgs> TimeLeftTickEvent;
		event EventHandler<ControllingEventArgs> ControllingEvent;
		TimeSpan OriginalTime { get; }
		bool IsRunning { get; }
		void DisableControl();
		void Start();
		void Stop();
	}

	public class ControllingEventArgs : EventArgs
	{
		public bool IsControlling { get; set; }
	}

	public class CountdownTickEventArgs : EventArgs
	{
		public TimeSpan TickTime { get; set; }
	}

	public class NewVelocityEventArgs : EventArgs
	{
		public double Velocity { get; set; }
	}

	public class NewDirectionEventArgs : EventArgs
	{
		public double Direction { get; set; }
	}
}