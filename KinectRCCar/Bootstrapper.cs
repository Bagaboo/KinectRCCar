using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace KinectRCCar
{
	class Bootstrapper : UnityBootstrapper, IDisposable
	{
		
		protected override void InitializeShell()
		{
			base.InitializeShell();
			Application.Current.MainWindow = (Shell)Shell;
			Application.Current.MainWindow.Show();
		}

		protected override DependencyObject CreateShell()
		{
			return Container.Resolve<Shell>();
		}

		protected override IModuleCatalog CreateModuleCatalog()
		{
			return new DirectoryModuleCatalog
			       	{
			       		ModulePath = @".\Modules"
			       	};
		}

		public void Dispose()
		{
			Container.Dispose();
		}
	}
}
