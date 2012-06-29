using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectRCCar.Kinect.Adaptors.Interfaces;
using Microsoft.Kinect;
using Microsoft.Practices.Unity;

namespace KinectRCCar.Kinect.Adaptors
{
	public class KinectSensorAdapterFactory : IKinectSensorAdapterFactory
	{
		private readonly IUnityContainer _container;

		public KinectSensorAdapterFactory(IUnityContainer container)
		{
			_container = container;
		}

		public IKinectSensorAdapter GetFirstKinectSensor()
		{
			KinectSensor kinectSensor = KinectSensor.KinectSensors.FirstOrDefault();
			
			if (kinectSensor == null)
				return null;
			
			IKinectSensorAdapter sensorAdapter = _container.Resolve<IKinectSensorAdapter>(new ParameterOverride("kinectSensor", kinectSensor));
			return sensorAdapter;
		}

		public event EventHandler<StatusChangedEventArgs> StatusChanged
		{
			add { KinectSensor.KinectSensors.StatusChanged += value; }
			remove { KinectSensor.KinectSensors.StatusChanged -= value; }
		}
	}
}
