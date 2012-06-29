using System.Management;
using KinectRCCar.CarController.Serial;
using KinectRCCar.Infrastructure.Interfaces;
using KinectRCCar.MjpegProcessor;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace KinectRCCar.CarController
{
	[Module(ModuleName = "CarControllerModule")]
	public class CarControllerModule : IModule
	{
		private readonly IUnityContainer _container;

		public CarControllerModule(IUnityContainer container)
		{
			_container = container;
		}

		public void Initialize()
		{
			_container
				.RegisterType<ICarControllerSettings,CarControllerSettings>()
				.RegisterType<ISerialPortAdapter, SerialPortAdapter>()
				.RegisterType<IWmiMbedHelper,WmiMbedHelper>(new ContainerControlledLifetimeManager())
				.RegisterType<IMbedService, MbedPortService>(new ContainerControlledLifetimeManager())
				.RegisterType<ICarControllerService, CarControllerService>(new ContainerControlledLifetimeManager())
				.RegisterType<MjpegDecoder>(new ContainerControlledLifetimeManager());
		}
	}
}
