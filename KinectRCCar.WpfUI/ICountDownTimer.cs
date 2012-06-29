using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectRCCar.WpfUI
{
	public interface ICountdownTimer
	{
		void Start();
		void Stop();
		void Reset();
		bool IsEnabled { get; set; }
		TimeSpan OriginalTime { get; }
		event EventHandler<CountdownEventEventArgs> CountdownTimerTick;
		event EventHandler<TimeElapsedEventArgs>TimeElaped;
	}

	public class CountdownEventEventArgs : EventArgs
	{
		public TimeSpan TimeLeft { get; set; }
	}

	public class TimeElapsedEventArgs : EventArgs
	{
		public DateTime SignalTime { get; set; }
	}
}
