using System;
using System.Management;

namespace KinectRCCar.CarController.Serial
{
	public class WmiMbedHelper : IWmiMbedHelper
	{
		private const string MBED_DEVICE_ID = "\"%USB\\\\VID_0D28&PID_0204&MI_01%\"";

		private readonly ManagementEventWatcher _managementEventWatcher;

		public WmiMbedHelper()
		{
			_managementEventWatcher = new ManagementEventWatcher();
		}

		public void Start()
		{
			if (IsMbedAttached())
			{
				OnMbedAttachedEvent(MbedAttachedStatus.Attached);
				ListenforMbedDetach();
			}
			else
			{
				OnMbedAttachedEvent(MbedAttachedStatus.Detached);
				ListenforMbedAttach();
			}
		}

		public void Stop()
		{
			StopListening();
		}

		private static bool IsMbedAttached()
		{
			bool isMbedAttached;

			WqlObjectQuery query = new WqlObjectQuery("SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE " + MBED_DEVICE_ID);

			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher())
			{
				managementObjectSearcher.Scope = new ManagementScope("root\\CIMV2");
				managementObjectSearcher.Query = query;
				isMbedAttached = managementObjectSearcher.Get().Count != 0;
			}
			return isMbedAttached;
		}
		
		private void ListenforMbedAttach()
		{
			WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.DeviceID LIKE " + MBED_DEVICE_ID);
			StartListening(query);
		}

		private void ListenforMbedDetach()
		{
			WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.DeviceID LIKE " + MBED_DEVICE_ID);
			StartListening(query);
		}

		private void StartListening(WqlEventQuery query)
		{
			StopListening();
			_managementEventWatcher.Query = query;
			_managementEventWatcher.EventArrived += ManagementEventWatcherOnEventArrived;
			_managementEventWatcher.Start();
		}

		private void StopListening()
		{
			if (_managementEventWatcher == null) 
				return;

			_managementEventWatcher.EventArrived -= ManagementEventWatcherOnEventArrived;
			_managementEventWatcher.Stop();
		}

		private void ManagementEventWatcherOnEventArrived(object sender, EventArrivedEventArgs eventArrivedEventArgs)
		{
			if (eventArrivedEventArgs.NewEvent.ClassPath.RelativePath == "__InstanceCreationEvent")
			{
				OnMbedAttachedEvent(MbedAttachedStatus.Attached );
				StopListening();
				ListenforMbedDetach();
			}

			if (eventArrivedEventArgs.NewEvent.ClassPath.RelativePath == "__InstanceDeletionEvent")
			{
				OnMbedAttachedEvent(MbedAttachedStatus.Detached );
				StopListening();
				ListenforMbedAttach();
			}
			
		}

		public event EventHandler<MbedAttachedEventArgs> MbedAttachedEvent;

		private void OnMbedAttachedEvent(MbedAttachedStatus status)
		{
			MbedAttachedEventArgs e = new MbedAttachedEventArgs
			                          	{
			                          		Status = status
			                          	};

			EventHandler<MbedAttachedEventArgs> handler = MbedAttachedEvent;
			if (handler != null) handler(this, e);
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
					StopListening();
					_managementEventWatcher.Dispose();
				}

				_disposed = true;
			}
		}
	}
}
