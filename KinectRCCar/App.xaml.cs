using System;
using System.Windows;

namespace KinectRCCar
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application, IDisposable
	{
		private Bootstrapper _bootstrapper;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			_bootstrapper = new Bootstrapper();
			_bootstrapper.Run();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			_bootstrapper.Dispose();
		}
		
		private bool _disposed;

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_bootstrapper.Dispose();
				}

				_disposed = true;
			}
		}
	}
}
