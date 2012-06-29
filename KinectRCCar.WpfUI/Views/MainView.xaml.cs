using System.Windows.Controls;
using KinectRCCar.WpfUI.ViewModels;

namespace KinectRCCar.WpfUI.Views
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : UserControl
	{
		public MainView()
		{
			InitializeComponent();
		}

		public MainView(MainViewModel viewModel):this()
		{
			layoutRoot.DataContext = viewModel;
		}
	}
}
