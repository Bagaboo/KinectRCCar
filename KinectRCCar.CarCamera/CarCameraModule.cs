using KinectRCCar.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace KinectRCCar.CarCamera
{
	[Module(ModuleName = "CarCameraModule")]
	public class CarCameraModule : IModule
	{
		private IUnityContainer _container;

		public CarCameraModule(IUnityContainer container)
		{
			_container = container;
		}

		public void Initialize()
		{
			_container.RegisterType<ICarCameraService, CarCameraService>(new ContainerControlledLifetimeManager());
		}
	}
}
