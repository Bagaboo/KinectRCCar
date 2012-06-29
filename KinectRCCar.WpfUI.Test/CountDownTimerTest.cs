using System;
using System.Timers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KinectRCCar.WpfUI.Test
{
	
	
	/// <summary>
	///This is a test class for CountDownTimerTest and is intended
	///to contain all CountDownTimerTest Unit Tests
	///</summary>
	[TestClass()]
	public class CountDownTimerTest
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
			
		}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		/// <summary>
		///A test for CountdownTimer Constructor
		///</summary>
		[TestMethod()]
		public void CountDownTimerConstructorTest()
		{
			TimeSpan time = new TimeSpan(); 
			TimeSpan interval = new TimeSpan(); 
			CountdownTimer target = new CountdownTimer(time, interval);
			Assert.IsNotNull(target);
			Assert.IsInstanceOfType(target, typeof(CountdownTimer));
		}

		/// <summary>
		///A test for DispatcherTimerOnTick
		///</summary>
		[TestMethod()]
		[DeploymentItem("KinectRCCar.WpfUI.dll")]
		public void DispatcherTimerOnTickTest()
		{
			CountdownTimer timer = new CountdownTimer(new TimeSpan(0, 0, 0, 5), new TimeSpan(0, 0, 0, 1));
			PrivateObject privateObject = new PrivateObject(timer);

			Assert.AreEqual(new TimeSpan(0, 0, 0, 5), (TimeSpan)privateObject.GetField("_startTime"));
			privateObject.Invoke("DispatcherTimerOnTick", new object[] {null,null});

			Assert.AreEqual(new TimeSpan(0, 0, 0, 4), (TimeSpan)privateObject.GetField("_startTime"));
		}
		[TestMethod]
		[DeploymentItem("KinectRCCar.WpfUI.dll")]
		public void DispatcherTimerTest()
		{
			CountdownTimer timer = new CountdownTimer(new TimeSpan(0, 0, 0, 5), new TimeSpan(0, 0, 0, 1));
			PrivateObject privateObject = new PrivateObject(timer);
			
			Timer testTimer = new Timer(5001);

			testTimer.Elapsed += (sender, args) =>
			                	{
			                		Assert.IsFalse((bool)privateObject.GetProperty("IsEnabled"));
									Assert.AreEqual(new TimeSpan(), (TimeSpan)privateObject.GetField("_startTime"));
			                	};

			Assert.AreEqual(new TimeSpan(0, 0, 0, 5), (TimeSpan)privateObject.GetField("_startTime"));
			Assert.IsFalse((bool)privateObject.GetProperty("IsEnabled"));
			privateObject.Invoke("Start");
			Assert.IsTrue((bool)privateObject.GetProperty("IsEnabled"));
			testTimer.Start();
		}
	}
}
