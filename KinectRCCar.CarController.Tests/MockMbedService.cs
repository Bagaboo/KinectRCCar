using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRCCar.CarController.Serial;

namespace KinectRCCar.CarController.Tests
{
	public class MockMbedService : IMbedService
	{
		public MockMbedService()
		{
			
		}

		public void Dispose()
		{
			Stop();
		}

		public virtual void Start()
		{
			Status = MbedStatus.Connected;
			OnMbedStatusChanged(new MbedStatusEventArgs() { Status = Status});
		}

		public virtual void Stop()
		{
			Status = MbedStatus.Disconnected;
			OnMbedStatusChanged(new MbedStatusEventArgs() { Status = Status });
		}

		public virtual string RPC(string function, params int[] arguments)
		{
			throw new NotImplementedException();
		}

		public bool IsRunning { get; private set; }

		public virtual MbedStatus Status { get; private set; }

		public virtual event EventHandler<MbedStatusEventArgs> MbedStatusChanged;

		public void OnMbedStatusChanged(MbedStatusEventArgs e)
		{
			EventHandler<MbedStatusEventArgs> handler = MbedStatusChanged;
			if (handler != null) handler(this, e);
		}
	}
}
