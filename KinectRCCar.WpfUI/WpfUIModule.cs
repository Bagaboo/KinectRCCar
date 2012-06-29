using KinectRCCar.WpfUI.ViewModels;
using KinectRCCar.WpfUI.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace KinectRCCar.WpfUI
{
	[Module(ModuleName = "WpfUIModule")]
	[ModuleDependency("KinectModule")]
	[ModuleDependency("CarControllerModule")]
	[ModuleDependency("CarCameraModule")]
	public class WpfUIModule : IModule
	{
		private IUnityContainer _container;
		private IRegionManager _regionManager;

		public WpfUIModule(IUnityContainer container, IRegionManager regionManager)
		{
			_container = container;
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_container
				.RegisterType<IWpfUISettings,WpfUISettings>()
				.RegisterType<ICountdownTimer, CountdownTimer>()
				.RegisterType<IControlLogic, ControlLogic>()
				.RegisterType<ViewModelBase, MainViewModel>()
				.RegisterType<MainView>();

			_regionManager.RegisterViewWithRegion("MainViewRegion", () => _container.Resolve<MainView>());

		}
	}
}
