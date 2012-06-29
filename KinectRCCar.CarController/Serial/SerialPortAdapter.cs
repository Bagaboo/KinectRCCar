using System;
using System.IO.Ports;

namespace KinectRCCar.CarController.Serial
{
	public class SerialPortAdapter : ISerialPortAdapter, IDisposable
	{
		private readonly SerialPort _serialPort;

		public SerialPortAdapter()
		{
			_serialPort = new SerialPort();
		}
		
		public static string[] GetPortNames()
		{
			return SerialPort.GetPortNames();
		}
		
		public string PortName
		{
			get { return _serialPort.PortName; }
			set { _serialPort.PortName = value; }
		}

		public int BaudRate
		{
			get { return _serialPort.BaudRate; }
			set { _serialPort.BaudRate = value; }
		}

		public int ReadTimeout
		{
			get { return _serialPort.ReadTimeout; }
			set { _serialPort.ReadTimeout = value; }
		}

		public int WriteTimeout
		{
			get { return _serialPort.WriteTimeout; }
			set { _serialPort.WriteTimeout = value; }
		}
		
		public bool IsOpen
		{
			get { return _serialPort.IsOpen; }
		}

		public void Open()
		{
			_serialPort.Open();
		}

		public void Close()
		{
			_serialPort.Close();
		}

		public void WriteLine(string text)
		{
			_serialPort.WriteLine(text);
		}

		public string ReadLine()
		{
			return _serialPort.ReadLine();
		}

		public event SerialErrorReceivedEventHandler ErrorReceived
		{
			add { _serialPort.ErrorReceived += value; }
			remove { _serialPort.ErrorReceived -= value; }
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
					_serialPort.Close();
					_serialPort.Dispose();
				}

				_disposed = true;
			}
		}
	}
}
