using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using KinectRCCar.Infrastructure.Interfaces;
using KinectRCCar.Kinect;
using KinectRCCar.WpfUI.Properties;
using Microsoft.Kinect;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;

namespace KinectRCCar.WpfUI.ViewModels
{
	public sealed class MainViewModel : ViewModelBase
	{
		private readonly IKinectService _kinectService;
		private readonly ICarControllerService _carControllerService;
		private readonly ICarCameraService _carCameraService;

		private readonly BitmapImage _testImage;

		private readonly IControlLogic _controlLogic;

		public MainViewModel()
		{
			_testImage = new BitmapImage(new Uri("../Images/VideoTestPattern.jpg", UriKind.Relative));
			CarCameraImage = _testImage;
			KinectCameraImage = _testImage;

			ConnectButtonContent = "Connect";
		}

		public MainViewModel(IKinectService kinectService,
		                     ICarControllerService carControllerService, ICarCameraService carCameraService, IControlLogic controlLogic)
			: this()
		{

			//Setup Camera
			_carCameraService = carCameraService;
			_carCameraService.ImageReady += (sender, args) => CarCameraImage = args.BitmapImage;

			//Setup Car Controller
			_carControllerService = carControllerService;
			CarControllerStatus = _carControllerService.Status;
			_carControllerService.CarControllerStatusChanged += CarControllerServiceOnCarControllerStatusChanged;

			//Setup Kinect
			_kinectService = kinectService;
			KinectSensorStatus = _kinectService.Status;
			_kinectService.KinectStatusChanged += KinectServiceOnKinectStatusChanged;
			_kinectService.SkeletonFrameReady += KinectServiceOnSkeletonFrameReady;
			_kinectService.ColorImageReady += (sender, args) => KinectCameraImage = args.BitmapSource;

			//setup Control logic (business logic)
			_controlLogic = controlLogic;
			_controlLogic.ControllingEvent += (sender, args) => IsControlling = args.IsControlling;
			_controlLogic.NewDirectionEvent += (sender, args) => Direction = args.Direction;
			_controlLogic.NewVelocityEvent += (sender, args) => Velocity = args.Velocity;
			_controlLogic.TimeLeftTickEvent += (sender, args) => CountDown = args.TickTime;
			CountDown = _controlLogic.OriginalTime;
			
			//Setup Commands
			RetryCommand = new DelegateCommand(Retry);
			CancelCommand = new DelegateCommand(Cancel);
			ConnectCommand = new DelegateCommand(Connect);
			SetKinectAngleCommand = new DelegateCommand(SetKinectAngle);
		}

		private void CarControllerServiceOnCarControllerStatusChanged(object sender, CarControllerStatusEventArgs e)
		{
			CarControllerStatus = e.Status;

			switch (CarControllerStatus)
			{
				case CarControllerStatus.Connected:
					break;
					// Car Controller just disconnected.
					// Stop all connections.
				case CarControllerStatus.Disconnected:
					if (!IsRunning)
						return;
					DeviceError(true);
					break;
			}
		}

		private void KinectServiceOnKinectStatusChanged(object sender, KinectStatusEventArgs e)
		{
			KinectSensorStatus = e.Status;

			switch (KinectSensorStatus)
			{
				case KinectStatus.Initializing:
					break;
				case KinectStatus.Connected:
					break;
				case KinectStatus.Undefined:
				case KinectStatus.Error:
				case KinectStatus.DeviceNotSupported:
				case KinectStatus.InsufficientBandwidth:
				case KinectStatus.NotPowered:
				case KinectStatus.NotReady:
				case KinectStatus.DeviceNotGenuine:
				case KinectStatus.Disconnected:
					if (!IsRunning)
						return;
					DeviceError(true);
					break;
			}
		}

		private void DeviceError(bool isError)
		{
			IsDeviceError = isError;
			if (isError)
				Disconnect();
		}

		private bool VerifyServicesRunning()
		{
			return _controlLogic.IsRunning;
		}

		private bool VerifyServicesConnected()
		{
			return _carControllerService.Status == CarControllerStatus.Connected &&
			       _kinectService.Status == KinectStatus.Connected;
		}

		private void StartServices()
		{
			// Enable control
			_controlLogic.Start();

			 //Enable video from the camera
			_carCameraService.CameraAddress = CarCameraAddress;
			_carCameraService.Username = CarCameraUsername;
			_carCameraService.Password = CarCameraPassword;
			_carCameraService.Start();
		}

		private void StopServices()
		{
			_controlLogic.Stop();
			_carCameraService.Stop();
		}

		private ICommand _connectCommand;

		public ICommand ConnectCommand
		{
			get { return _connectCommand; }
			private set
			{
				_connectCommand = value;
				RaisePropertyChanged("ConnectCommand");
			}
		}

		private bool _isConnectButtonChecked;

		public bool IsConnectButtonChecked
		{
			get { return _isConnectButtonChecked; }
			set
			{
				_isConnectButtonChecked = value;
				RaisePropertyChanged("IsConnectButtonChecked");
			}
		}

		private void Connect()
		{
			if (IsRunning)
				return;

			if (!VerifyServicesConnected())
			{
				IsConnectButtonChecked = false;
				DeviceError(true);
				return;
			}

			StartServices();
			Thread.Sleep(500);
			if (VerifyServicesRunning())
			{
				ConnectButtonContent = "Disconnect";
				ConnectCommand = new DelegateCommand(Disconnect);

				MaxKinectElevationAngle = _kinectService.MaximumElevation;
				MinKinectElevationAngle = _kinectService.MinimumElevation;
				if (_kinectService.IsRunning)
					KinectElevationAngle = _kinectService.ElevationAngle;

				IsRunning = true;
				IsConnectButtonChecked = true;
			}
			else
			{
				DeviceError(true);
			}
		}

		private void Disconnect()
		{
			_controlLogic.DisableControl();

			StopServices();

			KinectCameraImage = _testImage;
			CarCameraImage = _testImage;
			Skeletons = new Skeleton[0];
			KinectElevationAngle = 0;
			MaxKinectElevationAngle = 0;
			MinKinectElevationAngle = 10;

			ConnectButtonContent = "Connect";
			IsConnectButtonChecked = false;
			ConnectCommand = new DelegateCommand(Connect);
			IsRunning = false;
		}

		public ICommand SetKinectAngleCommand { get; private set; }

		private void SetKinectAngle()
		{
			_kinectService.ElevationAngle = KinectElevationAngle;
		}

		public ICommand RetryCommand { get; private set; }

		private void Retry()
		{
			if (!VerifyServicesConnected())
				return;

			DeviceError(false);
			Connect();
		}

		public ICommand CancelCommand { get; private set; }

		private void Cancel()
		{
			DeviceError(false);
		}

		private string _connectButtonContent;

		public string ConnectButtonContent
		{
			get { return _connectButtonContent; }
			set
			{
				_connectButtonContent = value;
				RaisePropertyChanged("ConnectButtonContent");
			}
		}

		private BitmapImage _carCameraImage;

		public BitmapImage CarCameraImage
		{
			get { return _carCameraImage; }
			set
			{
				_carCameraImage = value;
				RaisePropertyChanged("CarCameraImage");
			}
		}

		private BitmapSource _kinectCameraImage;

		public BitmapSource KinectCameraImage
		{
			get { return _kinectCameraImage; }
			set
			{
				_kinectCameraImage = value;
				RaisePropertyChanged("KinectCameraImage");
			}
		}

		private CarControllerStatus _carControllerStatus;

		public CarControllerStatus CarControllerStatus
		{
			get { return _carControllerStatus; }
			set
			{
				_carControllerStatus = value;
				RaisePropertyChanged("CarControllerStatus");
			}
		}

		private KinectStatus _kinectSensorStatus;

		public KinectStatus KinectSensorStatus
		{
			get { return _kinectSensorStatus; }
			set
			{
				_kinectSensorStatus = value;
				RaisePropertyChanged("KinectSensorStatus");
			}
		}

		private bool _isDeviceError;

		public bool IsDeviceError
		{
			get { return _isDeviceError; }
			set
			{
				_isDeviceError = value;
				RaisePropertyChanged("IsDeviceError");
			}
		}

		private Skeleton[] _skeletons;

		public Skeleton[] Skeletons
		{
			get { return _skeletons; }
			set
			{
				_skeletons = value;
				RaisePropertyChanged("Skeletons");
			}
		}

		private int _kinectElevationAngle;

		public int KinectElevationAngle
		{
			get { return _kinectElevationAngle; }
			set
			{
				_kinectElevationAngle = value;
				RaisePropertyChanged("KinectElevationAngle");
			}
		}

		private int _maxKinectElevationAngle;

		public int MaxKinectElevationAngle
		{
			get { return _maxKinectElevationAngle; }
			set
			{
				_maxKinectElevationAngle = value;
				RaisePropertyChanged("MaxKinectElevationAngle");
			}
		}

		private int _minKinectElevationAngle;

		public int MinKinectElevationAngle
		{
			get { return _minKinectElevationAngle; }
			set
			{
				_minKinectElevationAngle = value;
				RaisePropertyChanged("MinKinectElevationAngle");
			}
		}

		private string _carCameraUsername;

		public string CarCameraUsername
		{
			get { return _carCameraUsername; }
			set
			{
				_carCameraUsername = value;
				RaisePropertyChanged("CarCameraUsername");
			}
		}

		private string _carCameraPassword;

		public string CarCameraPassword
		{
			get { return _carCameraPassword; }
			set
			{
				_carCameraPassword = value;
				RaisePropertyChanged("CarCameraPassword");
			}
		}

		private Uri _carCameraAddress;

		public Uri CarCameraAddress
		{
			get { return _carCameraAddress; }
			set
			{
				_carCameraAddress = value;
				RaisePropertyChanged("CarCameraAddress");
			}
		}

		private double _direction;

		public double Direction
		{
			get { return _direction; }
			set
			{
				_direction = value;
				RaisePropertyChanged("Direction");
			}
		}

		private double _velocity;

		public double Velocity
		{
			get { return _velocity; }
			set
			{
				_velocity = value;
				RaisePropertyChanged("Velocity");
			}
		}

		private bool _isRunning;

		public bool IsRunning
		{
			get { return _isRunning; }
			set
			{
				_isRunning = value;
				RaisePropertyChanged("IsRunning");
			}
		}

		private bool _isControlling;
		public bool IsControlling
		{
			get { return _isControlling; }
			set
			{
				_isControlling = value;
				RaisePropertyChanged("IsControlling");
			}
		}

		private TimeSpan _countDown;

		public TimeSpan CountDown
		{
			get { return _countDown; }
			set
			{
				_countDown = value;
				RaisePropertyChanged("CountDown");
			}
		}

		// Kinect Skeleton event
		private void KinectServiceOnSkeletonFrameReady(object sender, SkeletonArrayReadyEventArgs e)
		{
			Skeletons = e.Skeletons;
		}
	}
}
