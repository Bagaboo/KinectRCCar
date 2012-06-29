using System.Threading;
using KinectRCCar.CarController;
using KinectRCCar.CarController.Serial;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Management;

namespace KinectRCCar.CarController.Tests
{
	
	
	/// <summary>
	///This is a test class for WmiMbedHelperTest and is intended
	///to contain all WmiMbedHelperTest Unit Tests
	///</summary>
	[TestClass()]
	[DeploymentItem("KinectRCCar.CarController.dll")]
	public class WmiMbedHelperTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		[TestInitialize()]
		public void MyTestInitialize()
		{
			_target = new WmiMbedHelper_Accessor();
		}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		private WmiMbedHelper_Accessor _target;

		/// <summary>
		///A test for WmiMbedHelper Constructor
		///</summary>
		[TestMethod()]
		public void WmiMbedHelperConstructorTest()
		{
			Assert.IsNotNull(_target);
			Assert.IsNotNull(_target._managementEventWatcher);
		}

		/// <summary>
		///A test for Dispose
		///</summary>
		[TestMethod()]
		public void DisposeTest()
		{
			_target.Dispose();
			Assert.IsNull(_target._managementEventWatcher);
			
		}

		/// <summary>
		///A test for IsMbedAttached
		///</summary>
		[TestMethod()]
		public void IsMbedAttachedTest()
		{

			bool expected = false;
			bool actual;
			actual = WmiMbedHelper_Accessor.IsMbedAttached();
			Assert.AreEqual(expected, actual);
			
		}
		
		/// <summary>
		///A test for OnMbedAttachedEvent
		///</summary>
		[TestMethod()]
		public void OnMbedAttachedEventTest()
		{
			MbedAttachedStatus expected = MbedAttachedStatus.Attached;
			
			_target.add_MbedAttachedEvent((sender, args) => Assert.AreEqual(expected, args.Status));
			_target.OnMbedAttachedEvent(expected);

			expected = MbedAttachedStatus.Detached;
			_target.add_MbedAttachedEvent((sender, args) => Assert.AreEqual(expected, args.Status));	
		}

		/// <summary>
		///A test for StopListening
		///</summary>
		[TestMethod()]
		public void StopListeningTest()
		{
			bool stopped = false;


			_target._managementEventWatcher.Stopped += (sender, args) => stopped = true;

			_target.Start();
			_target.Stop();
			Thread.Sleep(500);
			Assert.IsNotNull(_target._managementEventWatcher);
			Assert.IsTrue(stopped);

		}
	}
}
