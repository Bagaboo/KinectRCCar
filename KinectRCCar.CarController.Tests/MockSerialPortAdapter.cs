using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using KinectRCCar.CarController.Serial;

namespace KinectRCCar.CarController.Tests
{
	public class MockSerialPortAdapter : ISerialPortAdapter
	{
		public string PortName { get; set; }

		public int BaudRate { get; set; }

		public int ReadTimeout { get; set; }

		public int WriteTimeout { get; set; }

		public bool IsOpen { get; private set; }

		public void Open()
		{
			IsOpen = true;
		}

		public void Close()
		{
			IsOpen = false;
		}

		public void WriteLine(string text)
		{
			
		}

		public string ReadLine()
		{
			return string.Empty;
		}

		public event SerialErrorReceivedEventHandler ErrorReceived;

		public void OnErrorReceived(SerialErrorReceivedEventArgs e)
		{
			SerialErrorReceivedEventHandler handler = ErrorReceived;
			if (handler != null) handler(this, e);
		}

		public void Dispose()
		{
			
		}
	}
}
