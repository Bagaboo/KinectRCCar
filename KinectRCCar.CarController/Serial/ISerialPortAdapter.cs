using System;
using System.IO.Ports;

namespace KinectRCCar.CarController.Serial
{
	public interface ISerialPortAdapter : IDisposable
	{
		string PortName { get; set; }
		int BaudRate { get; set; }
		int ReadTimeout { get; set; }
		int WriteTimeout { get; set; }
		bool IsOpen { get; }
		void Open();
		void Close();
		void WriteLine(string text);
		string ReadLine();
		event SerialErrorReceivedEventHandler ErrorReceived;
	}
}
