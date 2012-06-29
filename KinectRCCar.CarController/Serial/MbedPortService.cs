using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.Win32;

namespace KinectRCCar.CarController.Serial
{
	public sealed class MbedPortService : IMbedService
	{
		private readonly IWmiMbedHelper _wmiMbedHelper;
		private readonly ISerialPortAdapter _serialPortAdapter;

		private readonly string _comPort = string.Empty;
		private readonly int _baudRate;

		private readonly ICarControllerSettings _settings;

		[InjectionConstructor]
		public MbedPortService(ISerialPortAdapter serialPortAdapter, IWmiMbedHelper wmiMbedHelper, ICarControllerSettings settings):this(serialPortAdapter, wmiMbedHelper, "COM3", 115200, settings){}
		
		public MbedPortService(ISerialPortAdapter serialPortAdapter, IWmiMbedHelper wmiMbedHelper, string comPort, int baudRate, ICarControllerSettings settings)
		{
			_serialPortAdapter = serialPortAdapter;

			_comPort = comPort;
			_baudRate = baudRate;

			_settings = settings;

			_wmiMbedHelper = wmiMbedHelper;
			_wmiMbedHelper.MbedAttachedEvent += WmiMbedHelperOnMbedAttachedEvent;
			_wmiMbedHelper.Start();
		}

		private void WmiMbedHelperOnMbedAttachedEvent(object sender, MbedAttachedEventArgs mbedAttachedEventArgs)
		{
			if (mbedAttachedEventArgs.Status == MbedAttachedStatus.Attached)
			{
				OnMbedStatusChanged(MbedStatus.Connected);
			}

			if (mbedAttachedEventArgs.Status == MbedAttachedStatus.Detached)
			{
				Stop();
				OnMbedStatusChanged(MbedStatus.Disconnected);
			}
		}

		#region Methods

		public void Start()
		{
			// don't initialize if we are already running or are any other state but the controller connected
			if (IsRunning || Status != MbedStatus.Connected)
				return;

			Stop();

			_serialPortAdapter.ErrorReceived += SerialPortAdapterOnErrorReceived;
			
			string comPort = string.IsNullOrEmpty(_comPort) ? GetMbedPortName() : _comPort;
			if (Status == MbedStatus.NoDriver || Status == MbedStatus.Disconnected)
			{
				return;
			}
			_serialPortAdapter.PortName = comPort;
			_serialPortAdapter.BaudRate = _baudRate;
			
			_serialPortAdapter.ReadTimeout = 500;
			_serialPortAdapter.WriteTimeout = 500;

			const int portOpenMaxAttempts = 10;
			int portOpenAttempts = 0;

			while (!IsRunning)
			{
				try
				{
					_serialPortAdapter.Open();
					IsRunning = true;
				}

				catch (UnauthorizedAccessException)
				{
					// Log, close, then try again
					if (portOpenAttempts++ < portOpenMaxAttempts)
					{
						Thread.Sleep(100);
						_serialPortAdapter.Close();
					}
					else
					{
						Stop();
						OnMbedStatusChanged(MbedStatus.Error);
					}
				}
				catch (Exception)
				{
					Stop();
					OnMbedStatusChanged(MbedStatus.Error);
					break;
				}
			}
			IsRunning = true;
		}

		private void SerialPortAdapterOnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			Debugger.Break();
			OnMbedStatusChanged(MbedStatus.Error);
			Stop();
		}

		public void Stop()
		{
			if (!_serialPortAdapter.IsOpen)
				return;
			
			RPC("Reset");

			_serialPortAdapter.Close();
			IsRunning = false;
		}

		public string RPC(string function, params int[] arguments)
		{
			string response = "128";

			if (_serialPortAdapter == null || _serialPortAdapter.IsOpen == false)
			{
				return response;
			}
			//throw new InvalidOperationException("The Serial port has not been initialized. Did you initialize the port?");

			
			string functionArguments = arguments.Aggregate(string.Empty, (current, argument) => string.Format("{0} {1}", current, argument));

			string rpcCommand = string.Format("/{0}/run {1}", function, functionArguments);

			try
			{
				_serialPortAdapter.WriteLine(rpcCommand);

				 response = _serialPortAdapter.ReadLine();
			}
			catch (Exception)
			{
				OnMbedStatusChanged(Status = MbedStatus.Disconnected);
				Stop();
			}
			Debug.WriteLine("Command sent to mbed: {0}", rpcCommand);

			return response;
		}


		private string GetMbedPortName(string strStartKey = null)
		{
			if (string.IsNullOrEmpty(strStartKey))
				strStartKey = _settings.PortSearchKey;

			// ...GetPortNames obtains a list of COM ports that are actually connected. Without using this
			// list the code will still try to connect to an mbed even if it isn't connected because the registry
			// seems to remember things even if they are unplugged
			string[] portNames = SerialPortAdapter.GetPortNames();
			RegistryKey currentKey = Registry.LocalMachine.OpenSubKey(strStartKey);
			
			if (currentKey == null)
				//throw new MbedLookupException("No mbed COM port driver found. Please install the latest Windows Serial Port Driver for the mbed.");
				OnMbedStatusChanged(Status = MbedStatus.NoDriver);

			string[] subKeyNames = currentKey.GetSubKeyNames();

			if (subKeyNames.Contains("Device Parameters"))
			{
				string portNameValue = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\" + strStartKey + "\\Device Parameters", "PortName", null);
				return portNames.Contains(portNameValue) ? portNameValue : null;
			}

			var result = (from strSubKey in subKeyNames
						  where GetMbedPortName(strStartKey + "\\" + strSubKey) != null
						  select GetMbedPortName(strStartKey + "\\" + strSubKey)).FirstOrDefault();

			if (result == null)
				//throw new MbedLookupException("Mbed not detected. Please verify the mbed is plugged in and shows in the Device Manager under \"Ports (COM & LPT)\"");
				OnMbedStatusChanged(MbedStatus.Disconnected );

			return result;
		}

		#endregion Methods

		#region Properties

		public bool IsRunning { get; private set; }

		public MbedStatus Status { get; private set; }
		
		#endregion

		#region Event Handlers

		public event EventHandler<MbedStatusEventArgs> MbedStatusChanged;

		public void OnMbedStatusChanged(MbedStatus status)
		{
			Status = status;
			MbedStatusEventArgs e = new MbedStatusEventArgs {Status = status};
			EventHandler<MbedStatusEventArgs> handler = MbedStatusChanged;
			if (handler != null) handler(this, e);
		}

		#endregion

		private bool _disposed;

		public void Dispose()
		{
			if(_disposed)
				return;

			Stop();
			_wmiMbedHelper.MbedAttachedEvent -= WmiMbedHelperOnMbedAttachedEvent;
			_wmiMbedHelper.Stop();
			_wmiMbedHelper.Dispose();
			_disposed = true;
		}
	}
}
