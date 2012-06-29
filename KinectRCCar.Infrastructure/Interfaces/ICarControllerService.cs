using System;

namespace KinectRCCar.Infrastructure.Interfaces
{
	/// <summary>
	/// <para>Inherit from this interface to roll your own Car Controller.</para>
	/// </summary>
	public interface ICarControllerService
	{
		#region Methods

		/// <summary>
		/// <para>Starts communication with the RC Controller.</para>
		/// </summary>
		void Start();
		/// <summary>
		/// <para>Stops communication with the RC Controller.</para>
		/// </summary>
		void Stop();

		/// <summary>
		/// <para>Resets the direction and velocity of the RC Car to 0. The straight forward and idle positions.
		/// </para>
		/// <para><see cref="SetDirection"/> and <see cref="SetVelocity"/> 
		/// on how to specify a specific direciton or velocity</para>
		/// </summary>
		void Reset();

		/// <summary>
		/// <para>Retrieve the set direction of the RC Car as a precentage of direction.</para>
		/// <para>A positive value represents a direction to the right.
		/// A negative value represents a direction to the left. 
		/// </para>
		///  </summary>
		/// <returns></returns>
		double GetDirection();

		/// <summary>
		/// <para>Sets the direction of the RC car based on a percentage of direction.</para> 
		/// <para>A negative <paramref name="directionPercent"/> represents a left oriented direction.
		/// A positive <paramref name="directionPercent"/> represents a right oriented direction.</para> 
		/// <remarks>Be sure to validate <c> -1.0 &gt;= <paramref name="directionPercent"/> &lt;= 1.0 </c></remarks>
		/// </summary>
		/// <param name="directionPercent">The percentage of the direction
		///  to set for the RC Car. Negative = Left. Positive = Right</param>
		void SetDirection(double directionPercent);

		/// <summary>
		/// <para>Retrieve the set velocity of the RC Car as a percentage of the throttle.</para>
		/// <para>A positive value represents the forward direction. 
		/// A negative value represents the reverse direction.</para>
		/// </summary>
		/// <returns>The velocity as a positive or negative percentage.</returns>
		double GetVelocity();

		/// <summary>
		/// <para>Sets the velocity of the RC car based on a percentage of throttle. </para>
		/// <para>A positive <paramref name="velocityPercent"/> represents the forward direction. 
		/// A negative <paramref name="velocityPercent"/> represents the reverse direction.</para>
		/// <remarks>Be sure to validate <c> -1.0 &gt;= <paramref name="velocityPercent"/> &lt;= 1.0 </c></remarks>
		/// </summary>
		/// <param name="velocityPercent">The percentage of throttle to set for the RC Car.</param>
		void SetVelocity(double velocityPercent);
		
		#endregion Methods

		#region Properties
		
		/// <summary>
		/// <para>Indicates the current connection status of the CarController. <seealso cref="CarControllerStatus"/></para>
		/// </summary>
		CarControllerStatus Status { get; }

		/// <summary>
		/// <para>Indicates if the CarControllerService is currently running and controlling communicating with the RC Car.</para>
		/// </summary>
		bool IsRunning { get; }

		#endregion Properties

		#region Events
		

		/// <summary>
		/// <para>Should fire whenever the Car Controller is connected or disconnected from the computer. 
		/// <see cref="CarControllerStatus"/> for the list of statuses that can be returned.</para>
		/// </summary>
		event EventHandler<CarControllerStatusEventArgs> CarControllerStatusChanged;

		#endregion Events
	}

	/// <summary>
	/// <para>The given status of the Car Controller</para>
	/// </summary>
	public enum CarControllerStatus
	{
		/// <summary>
		/// <para>No status has been determined for the Car Controller.</para>
		/// </summary>
		Undefined,
		/// <summary>
		/// <para>The Car Controller is connected to the computer and is ready to be used.</para>
		/// </summary>
		Connected,
		/// <summary>
		/// <para>The Car Controller is not connected to the computer.</para>
		/// </summary>
		Disconnected
	}

	/// <summary>
	/// <para>Status Event Arguments</para>
	/// </summary>
	public class CarControllerStatusEventArgs : EventArgs
	{
		/// <summary>
		/// <para>The Status for the Kinect being used.</para>
		/// </summary>
		public CarControllerStatus Status { get; set; }
	}
}
