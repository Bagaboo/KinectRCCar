using System;
using System.Windows.Threading;

namespace KinectRCCar.WpfUI
{
	public class CountdownTimer : ICountdownTimer
	{
		private readonly DispatcherTimer _dispatcherTimer;
		private readonly TimeSpan _originalTime;
		private TimeSpan _startTime;

		public CountdownTimer(TimeSpan time, TimeSpan updateInterval)
		{
			_dispatcherTimer = new DispatcherTimer();

			if (updateInterval > time)
				throw new ArgumentException("updateInterval cannot be greater than time","updateInterval");
			
			_dispatcherTimer.Interval = updateInterval;
			_dispatcherTimer.Tick += DispatcherTimerOnTick;
			_startTime = time;
			_originalTime = time;
		}
		
		public void Start()
		{
			_dispatcherTimer.Start();
		}

		public void Stop()
		{
			_dispatcherTimer.Stop();
		}

		public void Reset()
		{
			_dispatcherTimer.Stop();
			_startTime = OriginalTime;
			OnCountDownTimerTick(_startTime);
		}
		
		public bool IsEnabled
		{
			get { return _dispatcherTimer.IsEnabled; }
			set { _dispatcherTimer.IsEnabled = value; }
		}

		public TimeSpan OriginalTime
		{
			get { return _originalTime; }
		}

		private void DispatcherTimerOnTick(object sender, EventArgs eventArgs)
		{
			if (_startTime == TimeSpan.Zero)
			{
				OnTimeElapsed(new TimeElapsedEventArgs() { SignalTime = DateTime.Now});
				Stop();
			}
			else
			{
				_startTime -= _dispatcherTimer.Interval;
				OnCountDownTimerTick(_startTime);
			}
		}
		
		public event EventHandler<CountdownEventEventArgs> CountdownTimerTick;

		private void OnCountDownTimerTick(TimeSpan timerTick)
		{
			if (CountdownTimerTick != null) 
				CountdownTimerTick(this, new CountdownEventEventArgs { TimeLeft = timerTick});
		}

		public event EventHandler<TimeElapsedEventArgs> TimeElaped;

		public void OnTimeElapsed(TimeElapsedEventArgs args)
		{
			if (TimeElaped != null)
				TimeElaped(this, args);
		}
	}
	
	
}
