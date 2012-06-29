using System;
using KinectRCCar.CarController.Properties;
using KinectRCCar.CarController.Serial;
using KinectRCCar.Infrastructure.Interfaces;

namespace KinectRCCar.CarController
{
	public class CarControllerService : ICarControllerService, IDisposable
	{
		private readonly IMbedService _mbedService;

		private readonly ICarControllerSettings _settings;

		public CarControllerService(IMbedService portService, ICarControllerSettings settings)
		{
			Status = CarControllerStatus.Disconnected;
			_mbedService = portService;
			_mbedService.MbedStatusChanged += MbedServiceOnMbedStatusChanged;

			_settings = settings;

			if (_mbedService.Status == MbedStatus.Connected)
				OnCarControllerStatusChanged(CarControllerStatus.Connected);
		}

		private void MbedServiceOnMbedStatusChanged(object sender, MbedStatusEventArgs mbedStatusEventArgs)
		{
			switch (mbedStatusEventArgs.Status)
			{
				case MbedStatus.Connected:
					OnCarControllerStatusChanged(CarControllerStatus.Connected);
					break;
				case MbedStatus.Disconnected:
				case MbedStatus.Error:
					OnCarControllerStatusChanged(CarControllerStatus.Disconnected);
					Stop();
					break;
			}
		}

		public void Start()
		{
			if (IsRunning || Status != CarControllerStatus.Connected) return;

			_mbedService.Start();

			if (_mbedService.IsRunning)
			{
				IsRunning = true;
			}
		}

		public void Stop()
		{
			if (!IsRunning) 
				return;

			Reset();
			_mbedService.Stop();
			IsRunning = false;
		}

		public CarControllerStatus Status { get; private set; }

		public bool IsRunning { get; private set; }

		public event EventHandler<CarControllerStatusEventArgs> CarControllerStatusChanged;

		public void OnCarControllerStatusChanged(CarControllerStatus status)
		{
			Status = status;
			EventHandler<CarControllerStatusEventArgs> handler = CarControllerStatusChanged;
			if (handler != null) handler(this, new CarControllerStatusEventArgs { Status = status});
		}

		public void Reset()
		{
			_mbedService.RPC("Reset");
		}

		public double GetDirection()
		{
			int direction = Read(ControlPot.Direction);
			double directionPercent;

			directionPercent = ((double)direction - _settings.DirectionMidpoint)/_settings.DirectionRange;

			return directionPercent;
		}

		public void SetDirection(double directionPercent)
		{
			if (directionPercent < -1.0 || directionPercent > 1.0)
				throw new ArgumentOutOfRangeException("directionPercent", "Must be a percentage between -1 and 1.");

			int directionValue = Convert.ToInt32(_settings.DirectionRange * directionPercent);

			directionValue = _settings.DirectionMidpoint + directionValue;

			Write(ControlPot.Direction, directionValue);
		}

		public double GetVelocity()
		{
			int velocity = Read(ControlPot.Velocity);
			double velocityPercent;

			if (velocity >= _settings.ForwardMinimum)

				velocityPercent = ((double)velocity - _settings.ForwardMinimum) / (_settings.ForwardMaximum - _settings.ForwardMinimum);
			
			else if (velocity <= _settings.ReverseMinimum)

				velocityPercent = ((double)_settings.ReverseMinimum - velocity) / (_settings.ReverseMaximum - _settings.ReverseMinimum);
			else
				velocityPercent = 0.0;

			return velocityPercent;
		}

		public void SetVelocity(double velocityPercent)
		{
			if (velocityPercent < -1.0 || velocityPercent > 1.0)
				throw new ArgumentOutOfRangeException("velocityPercent", "Must be a percentage between -1 and 1.");

			int velocityValue;

			if (velocityPercent >= 0)
			{
				velocityValue = Convert.ToInt32((_settings.ForwardMaximum - _settings.ForwardMinimum)*velocityPercent);
	
				velocityValue = velocityValue + _settings.ForwardMinimum;
			}
			else
			{
				velocityValue = Convert.ToInt32((_settings.ReverseMinimum - _settings.ReverseMaximum)*velocityPercent);
				velocityValue = velocityValue + _settings.ReverseMinimum;
			}

			Write(ControlPot.Velocity, velocityValue);
		}

		public int Read(ControlPot pot)
		{
			string response = _mbedService.RPC("Read", (int)pot);

			int intResponse = Int32.Parse(response);

			return intResponse;
		}

		public void Write(ControlPot pot, int value)
		{
			_mbedService.RPC("Write", (int)pot, value);
			// used to turn the led off on the mbed.
			Read(pot);
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
					Stop();
					_mbedService.Dispose();
				}

				_disposed = true;
			}
		}
	}

	public enum ControlPot
	{
		Direction,
		Velocity
	}
}
