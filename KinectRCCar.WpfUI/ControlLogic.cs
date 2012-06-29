using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using KinectRCCar.Infrastructure.Interfaces;
using KinectRCCar.Kinect;
using KinectRCCar.WpfUI.Properties;

using Microsoft.Kinect;
using Microsoft.Practices.Unity;

namespace KinectRCCar.WpfUI
{
	public class ControlLogic : IControlLogic, IDisposable
	{
		private readonly IUnityContainer _container;

		private readonly IKinectService _kinectService;
		private readonly ICarControllerService _carControllerService;
		private ICountdownTimer _countdownTimer;

		
		private IWpfUISettings _settings;

		private Task _task;

		private CancellationTokenSource _cancellationTokenSource;
		
		public ControlLogic(IKinectService kinectService, ICarControllerService carControllerService, IWpfUISettings settings, IUnityContainer container)
		{
			_kinectService = kinectService;
			_carControllerService = carControllerService;
			_container = container;
			_settings = settings;

			_kinectService.SkeletonFrameReady += KinectServiceOnSkeletonFrameReady;
			
			SetupCountdownTimer();
		}

		private void SetupCountdownTimer()
		{
			_countdownTimer = _container.Resolve<ICountdownTimer>(new ParameterOverrides
			                                                      	{
			                                                      		{"time", TimeSpan.FromSeconds(_settings.CountdownTime)},
			                                                      		{"updateInterval", TimeSpan.FromMilliseconds(_settings.CountdownInterval)}
			                                                      	});

			_countdownTimer.CountdownTimerTick += (sender, e) => OnTimeLeftTick(e.TimeLeft);
			_countdownTimer.TimeElaped += (sender, args) => EnableControl();
		}

		private bool _isControlling;

		public void Start()
		{
			if (_carControllerService.Status != CarControllerStatus.Connected || _kinectService.Status != KinectStatus.Connected)
				return;

			_cancellationTokenSource = new CancellationTokenSource();
			_cancellationTokenSource.Token.Register(StopControl);
			
			_task = new Task(StartControl, _cancellationTokenSource.Token);
			_task.Start();
		}

		private void StartControl()
		{
			_carControllerService.Start();
			_kinectService.Start();
		}

		private void StopControl()
		{
			_kinectService.Stop();
			_carControllerService.Start();
		}

		public void Stop()
		{
			if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested) 
				return;

			_cancellationTokenSource.Cancel();
		}

		public bool IsRunning
		{
			get { return _carControllerService.IsRunning && _kinectService.IsRunning; }
		}

		private void EnableControl()
		{
			if (_isControlling) return;

			_countdownTimer.Reset();
			OnControllingEvent(true);
			_isControlling = true;
		}

		public void DisableControl()
		{
			// reset countdown timer
			CountDownTimerReset();

			if (!_isControlling) return;

			OnControllingEvent(false);
			_neutralDistanceFromBody = null;
			_carControllerService.Reset();
			OnNewDirectionEvent(_carControllerService.GetDirection());
			OnNewVelocityEvent(_carControllerService.GetVelocity());
			_isControlling = false;

			
		}

		public TimeSpan OriginalTime
		{
			get { return _countdownTimer.OriginalTime; }
		}

		private void CountDownTimerReset()
		{
			_countdownTimer.Reset();
			OnTimeLeftTick(_countdownTimer.OriginalTime);
		}

		public event EventHandler<NewDirectionEventArgs> NewDirectionEvent;

		private void OnNewDirectionEvent(double direction)
		{
			EventHandler<NewDirectionEventArgs> handler = NewDirectionEvent;
			if (handler != null) handler(this, new NewDirectionEventArgs { Direction = direction });
		}

		public event EventHandler<NewVelocityEventArgs> NewVelocityEvent;

		private void OnNewVelocityEvent(double velocity)
		{
			EventHandler<NewVelocityEventArgs> handler = NewVelocityEvent;
			if (handler != null) handler(this, new NewVelocityEventArgs { Velocity = velocity });
		}

		public event EventHandler<CountdownTickEventArgs> TimeLeftTickEvent;

		private void OnTimeLeftTick(TimeSpan tickTime)
		{
			EventHandler<CountdownTickEventArgs> handler = TimeLeftTickEvent;
			if (handler != null) handler(this, new CountdownTickEventArgs { TickTime = tickTime });
		}

		public event EventHandler<ControllingEventArgs> ControllingEvent;

		private void OnControllingEvent(bool isControlling)
		{
			
			EventHandler<ControllingEventArgs> handler = ControllingEvent;
			if (handler != null) handler(this, new ControllingEventArgs { IsControlling = isControlling });
		}

		#region KinectSkeletonEvent

		// Kinect Skeleton event
		private void KinectServiceOnSkeletonFrameReady(object sender, SkeletonArrayReadyEventArgs e)
		{
			ControlCar(e.Skeletons);
		}

		// Used to keep track of who is controlling the car
		private int _trackedId;

		// Parse the skeleton for use to control the car
		private void ControlCar(Skeleton[] skeletons)
		{
			// Get the closest tracked user
			Skeleton trackedSkeleton = skeletons.GetFirstTrackedSkeleton();

			// don't do anything if there is no one to track
			if (trackedSkeleton == null)
			{
				DisableControl();
				return;
			}

			// lost track of the previous user
			// stop controlling the car and start tracking
			// new user
			if (trackedSkeleton.TrackingId != _trackedId)
			{
				DisableControl();

				_trackedId = trackedSkeleton.TrackingId;
			}

			// get a vector representing the hands for easier coding
			Vector3D leftHand = GetJointVector(JointType.HandLeft, trackedSkeleton);

			Vector3D rightHand = GetJointVector(JointType.HandRight, trackedSkeleton);

			// Only continue if the hands are not crossed
			if (leftHand.X >= rightHand.X) return;

			// find the mindpoint between the two hands
			Vector3D handsMidpoint = new Vector3D((rightHand.X + leftHand.X)/2.0, 
												  (rightHand.Y + leftHand.Y)/2.0,
			                                      (rightHand.Z + leftHand.Z)/2.0);

			// get the vertical position of the spine 
			double spineYPosition = trackedSkeleton.Joints[JointType.Spine].Position.Y;

			// check if it is okay to control the car
			EnableDisableControl(handsMidpoint, spineYPosition, leftHand, rightHand);

			// if we are not controlling then return otherwise continue to processing.
			if (!_isControlling)
				return;

			// calculate the direction for the car
			CalculateDirection(leftHand, handsMidpoint);

			// calculate the velocity of the car
			CalculateVelocity(handsMidpoint.Z, trackedSkeleton.Position.Z);
		}

		/// <summary>
		/// Checks to see if the hands are in the starting position and in the "play area"
		/// before enabling control. Control is disabled if the hands drop outside the "play area.
		/// </summary>
		/// <param name="handsMidpoint">Vector representing the 3D midpoint of the hands.</param>
		/// <param name="spineYPosition">The Y position of the spine.</param>
		/// <param name="leftHand">Vector representing the 3D position of the left hand.</param>
		/// <param name="rightHand">Vector representing the 3D position of the right hand.</param>
		/// <returns>Whether the user can control the car or not.</returns>
		private void EnableDisableControl(Vector3D handsMidpoint, double spineYPosition, Vector3D leftHand, Vector3D rightHand)
		{
			// if we are not controlling
			if (!_isControlling)
			{
				// if the hands are horizontal and in the play area
				if (HandsStable(leftHand, rightHand) && HandsInPlayArea(handsMidpoint, spineYPosition))
				{
					// if the timer hasn't already been started
					if (!_countdownTimer.IsEnabled)
					{
						// start the timer
						_countdownTimer.Start();
					}
				}
					// if the hands are not horizontal or in the play area
				else
				{
					// reset
					DisableControl();
				}
			}
			// if we are controlling
			else
			{
				// if the hands are not in the play area
				if (!HandsInPlayArea(handsMidpoint, spineYPosition))
				{
					// stop controlling
					DisableControl();
				}
			}
		}

		// Checks to see if the hands have the same horizontal position 
		// and distance from the kinect.
		private bool HandsStable(Vector3D leftHand, Vector3D rightHand)
		{
			// are the hands horizontal
			bool areHorizontal = Math.Abs(leftHand.Y - rightHand.Y) < _settings.HorizontalHandThreshold;

			// are the hands depth equal
			bool areZStable = Math.Abs(leftHand.Z - rightHand.Z) < _settings.ZHandThreshold;
			return areHorizontal && areZStable;
		}

		// Checks if the hands are in the play area, defined as being
		// a distance above the Y position of the spine.
		private bool HandsInPlayArea(Vector3D handsMidpoint, double spinePositionY)
		{
			// Check if the both hands are below the base plane

			//defined as above the spine Y point that is the bottom of the play area
			double basePlane = spinePositionY + _settings.DistanceAboveSpineY;

			// Are the hands above the base plane of the play area
			bool handsAbovePlayAreaPlane = handsMidpoint.Y > basePlane;

			return handsAbovePlayAreaPlane;
		}

		private static Vector3D GetJointVector(JointType joint, Skeleton sd)
		{
			Vector3D newVector = new Vector3D
			                     	{
			                     		X = sd.Joints[joint].Position.X,
			                     		Y = sd.Joints[joint].Position.Y,
			                     		Z = sd.Joints[joint].Position.Z
			                     	};

			return newVector;
		}

		private double _previousDirection;

		//private double _lastCalculatedRadian;
		
		/// <summary>
		/// Calculates the Direction to have the car go.
		/// The direction is calculated as a percentage of the angle between the
		/// position of the <paramref name="leftHand"/> and 0.
		/// The calculated angle is then divided by Pi/2 to determine the direction
		/// as a percentage. 
		/// A positive direction is a direction to the right.
		/// A negative position is a direction to the left.
		/// </summary>
		/// <param name="leftHand">The hand to be used to calculate the direction.
		/// Note: this must be the lefthand or the calculation will be reversed.</param>
		/// <param name="handsMidpoint">The midpoint of the hands to determine if the
		/// direction is left or right based on if the <paramref name="leftHand"/> is
		/// above the midpoint or below.</param>
		private void CalculateDirection(Vector3D leftHand, Vector3D handsMidpoint)
		{
			double oppositeLength = leftHand.Y - handsMidpoint.Y;
			double adjacentLength = leftHand.X - handsMidpoint.X;

			// find the angle in degrees the hands are at
			double handTurnRadians = -(Math.Atan(oppositeLength/adjacentLength));

			// only update if the radians change by specified radians

			//if (Math.Abs(handTurnRadians - _lastCalculatedRadian) < CALULATED_RADIAN_TOLERANCE)
			//    return;

			//_lastCalculatedRadian = handTurnRadians;

			// if the angle of the hands is with in the threshold 
			// then set the direction of the vehicle appropriately
			if (handTurnRadians < -Settings.Default.MaxRadians || handTurnRadians > Settings.Default.MaxRadians)
				return;

			// calculate the direction as a percentage of the angle of the lefthand and Pi/2 (90 degress vertical).
			double direction = handTurnRadians/(Math.PI/2);
			Debug.WriteLine("Direction: {0}", direction);

			// make sure direction isn't larger than 100% either direction.
			// not sure if needed but just in case;
			if (direction > 1.0 || direction < -1.0)
				return;

			// only update the direction if the direction is different by 10%
			// from the previous direction
			if (Math.Abs(direction - _previousDirection) <= _settings.DirectionDeltaThreshold)
				return;

			_previousDirection = direction;
			_carControllerService.SetDirection(direction);
			OnNewDirectionEvent(direction);
		}

		// The z position the neutral position is from the body
		// set by the users hand position when control is enabled.
		private double? _neutralDistanceFromBody;

		private double _previousVelocity;
		
		/// <summary>
		/// Calculates the velocity of the car.
		/// The velocity is calculated as a percentage of the distance the
		/// hands are from a set neutral point.
		/// The is able to move back and forth as the neutral point is calculated
		/// based on the distance the neutral point is from the user.
		/// </summary>
		/// <param name="handsMidpointZ">The distance the hands are from the Kinect.</param>
		/// <param name="positionZ">The distnace the user is from the Kinect.</param>
		private void CalculateVelocity(double handsMidpointZ, double positionZ)
		{
			// if there is no neutral postion. Set it to be the handsMidpointZ.
			// Should only calculate once.
			// _neutralDistanceFromBody is reset to null in the
			// DisableControl() method.
			if (!_neutralDistanceFromBody.HasValue)
			{
				_neutralDistanceFromBody = positionZ - handsMidpointZ;
			}

			// calculate the position of neutral relative to the Kinect
			double throttleNeutralPosition = positionZ - _neutralDistanceFromBody.Value;

			// calculate distance the hands are from the neutral position
			double handDistanceFromNeutral = handsMidpointZ - throttleNeutralPosition;

			// calculate the velocity based on the ratio of the hand distance from neutral
			// to the maximum velocity range.
			double velocity = handDistanceFromNeutral/Settings.Default.MaxVelocityRange;

			// invert so forward direction is positive
			velocity = -velocity;

			Debug.WriteLine("Velocity: {0}", velocity);

			// Don't update the velocity if the velocity is greater than +-100%.
			if (velocity > 1.0 || velocity < -1.0) return;

			// only continue if the velocity changes more than 1%
			if (Math.Abs(velocity - _previousVelocity) <= _settings.VelocityDeltaThreshold)
				return;

			_carControllerService.SetVelocity(velocity);
			_previousVelocity = velocity;
			OnNewVelocityEvent(velocity);
		}

		#endregion KinectSkeletonEvent

		private bool _disposed;

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected  virtual void Dispose(bool disposing)
		{
			
			if (!_disposed)
			{
				if (disposing)
				{
					_cancellationTokenSource.Cancel();
					_cancellationTokenSource.Dispose();
					_task.Dispose();
				}
			}

			_disposed = true;
		}
	}
}
