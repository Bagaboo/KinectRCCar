using KinectRCCar.Infrastructure.Interfaces;
using KinectRCCar.Kinect.Adaptors;
using KinectRCCar.Kinect.Adaptors.Interfaces;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace KinectRCCar.Kinect
{
	[Module(ModuleName = "KinectModule")]
	public class KinectModule : IModule
	{
		private IUnityContainer _container;

		public KinectModule(IUnityContainer container)
		{
			_container = container;
		}

		public void Initialize()
		{
			_container
				.RegisterType<IKinectSettings,KinectSettings>()
				.RegisterType<IKinectSensorAdapterFactory, KinectSensorAdapterFactory>()
				.RegisterType<IColorImageStreamAdapter, ColorImageStreamAdapter>()
				.RegisterType<ISkeletonStreamAdapter, SkeletonStreamAdapter>()
				.RegisterType<IKinectSensorAdapter, KinectSensorAdapter>()
				.RegisterType<IKinectService, KinectService>(new ContainerControlledLifetimeManager());
		}
	}
}
