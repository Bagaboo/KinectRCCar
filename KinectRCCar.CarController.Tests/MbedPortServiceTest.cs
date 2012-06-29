using System;
using System.Management;
using KinectRCCar.CarController.Serial;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace KinectRCCar.CarController.Tests
{
	
	
	/// <summary>
	///This is a test class for MbedPortServiceTest and is intended
	///to contain all MbedPortServiceTest Unit Tests
	///</summary>
	[TestClass()]
	public class MbedPortServiceTest
	{
		private static MbedPortService_Accessor _mbedPortServiceAccessor;

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
		////Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//    _mbedPortServiceAccessor = new MbedPortService();
		//    _mbedPortServiceAccessor.Start();
		//}
		
		////Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//    _mbedPortServiceAccessor.Stop();
		//    _mbedPortServiceAccessor.Dispose();
		//    _mbedPortServiceAccessor = null;
		//}
		#endregion

		//// Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
			
		//}


		//// Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		
		[TestMethod]
		public void RPC_Constructor()
		{
			Mock<ISerialPortAdapter> mockSerialPortAdapter = new Mock<ISerialPortAdapter>(MockBehavior.Strict);
			
			Mock<IWmiMbedHelper> mockWmiMbedHelper = new Mock<IWmiMbedHelper>(MockBehavior.Strict);
			mockWmiMbedHelper.Setup(m => m.Start());

			Mock<ICarControllerSettings> mockCarControllerSettings = new Mock<ICarControllerSettings>(MockBehavior.Strict);


			_mbedPortServiceAccessor = new MbedPortService_Accessor(mockSerialPortAdapter.Object, mockWmiMbedHelper.Object, mockCarControllerSettings.Object);

			Assert.IsNotNull(_mbedPortServiceAccessor._wmiMbedHelper);
			Assert.IsNotNull(_mbedPortServiceAccessor._serialPortAdapter);

			mockSerialPortAdapter.Verify();
			mockWmiMbedHelper.Verify();
		}


		[TestMethod]
		[TestCategory("PortTest")]
		public void RPC_Start()
		{
			Mock<ISerialPortAdapter> mockSerialPortAdapter = new Mock<ISerialPortAdapter>(MockBehavior.Strict);
			mockSerialPortAdapter.Setup(m => m.Open());
			mockSerialPortAdapter.Setup(m => m.Close());
			mockSerialPortAdapter.SetupProperty(m => m.PortName);
			mockSerialPortAdapter.SetupProperty(m => m.BaudRate);
			mockSerialPortAdapter.SetupProperty(m => m.ReadTimeout);
			mockSerialPortAdapter.SetupProperty(m => m.WriteTimeout);
			mockSerialPortAdapter.SetupGet(m => m.IsOpen).Returns(false);


			Mock<IWmiMbedHelper> mockWmiMbedHelper = new Mock<IWmiMbedHelper>(MockBehavior.Strict);
			mockWmiMbedHelper.Setup(m => m.Start());

			Mock<ICarControllerSettings> mockCarControllerSettings = new Mock<ICarControllerSettings>(MockBehavior.Strict);

			_mbedPortServiceAccessor = new MbedPortService_Accessor(mockSerialPortAdapter.Object, mockWmiMbedHelper.Object, mockCarControllerSettings.Object);
			_mbedPortServiceAccessor.Status = MbedStatus.Connected;
			_mbedPortServiceAccessor.Start();

			Assert.IsTrue(_mbedPortServiceAccessor.IsRunning);
			mockSerialPortAdapter.Verify();
		}

		[TestMethod]
		[TestCategory("PortTest")]
		public void RPC_Pot0Read_Returns_128()
		{
			
			int expected = 128;
			string function = "Read";

			string resultString = _mbedPortServiceAccessor.RPC(function, 0);

			int resultInt = Convert.ToInt32(resultString);

			Assert.AreEqual(expected, resultInt);
		}

		[TestMethod]
		[TestCategory("PortTest")]
		public void RPC_Pot1Read_Returns128()
		{
			int expected = 128;
			string function = "Read";

			string resultString = _mbedPortServiceAccessor.RPC(function, 1);

			int resultInt = Convert.ToInt32(resultString);

			Assert.AreEqual(expected, resultInt);
		}

		[TestMethod]
		[TestCategory("PortTest")]
		public void RPC_Pot0Write_Returns0()
		{
			int expected = 0;
			string functionW = "Write";
			int[] parametersW = {0, expected};

			string resultStringW = _mbedPortServiceAccessor.RPC(functionW, parametersW);

			int resultIntW = Convert.ToInt32(resultStringW);

			Assert.AreEqual(1, resultIntW);

			string functionR = "Read";

			int[] parametersR = {expected};

			string resultStringR = _mbedPortServiceAccessor.RPC(functionR, parametersR);

			int resultIntR = Convert.ToInt32(resultStringR);

			Assert.AreEqual(expected, resultIntR);
		}

		[TestMethod]
		[TestCategory("PortTest")]
		public void RPC_POT0WriteOutofBounds_Returns128()
		{
			int expected = 128;
			string functionW = "Write";
			int[] parametersW = { 0, -1 };

			string resultStringW = _mbedPortServiceAccessor.RPC(functionW, parametersW);

			int resultIntW = Convert.ToInt32(resultStringW);

			Assert.AreEqual(0, resultIntW);

			string functionR = "Read";

			int[] parametersR = { 0, expected };

			string resultStringR = _mbedPortServiceAccessor.RPC(functionR, parametersR);

			int resultIntR = Convert.ToInt32(resultStringR);

			Assert.AreEqual(expected, resultIntR);
		}



		#region PredefinedTests
		///// <summary>
		/////A test for MbedPortService Constructor
		/////</summary>
		//[TestMethod()]
		//public void MbedPortServiceConstructorTest()
		//{
		//    string comPort = string.Empty; // TODO: Initialize to an appropriate value
		//    int baudRate = 0; // TODO: Initialize to an appropriate value
		//    MbedPortService target = new MbedPortService(comPort, baudRate);
		//    Assert.Inconclusive("TODO: Implement code to verify target");
		//}

		///// <summary>
		/////A test for Dispose
		/////</summary>
		//[TestMethod()]
		//public void DisposeTest()
		//{
		//    string comPort = string.Empty; // TODO: Initialize to an appropriate value
		//    int baudRate = 0; // TODO: Initialize to an appropriate value
		//    MbedPortService target = new MbedPortService(comPort, baudRate); // TODO: Initialize to an appropriate value
		//    target.Dispose();
		//    Assert.Inconclusive("A method that does not return a value cannot be verified.");
		//}

		///// <summary>
		/////A test for GetMbedPortName
		/////</summary>
		//[TestMethod()]
		//[DeploymentItem("KinectRCCar.CarController.dll")]
		//public void GetMbedPortNameTest()
		//{
		//    string strStartKey = string.Empty; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = MbedPortService_Accessor.GetMbedPortName(strStartKey);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		///// <summary>
		/////A test for Start
		/////</summary>
		//[TestMethod()]
		//public void InitializeTest()
		//{
		//    string comPort = string.Empty; // TODO: Initialize to an appropriate value
		//    int baudRate = 0; // TODO: Initialize to an appropriate value
		//    MbedPortService target = new MbedPortService(comPort, baudRate); // TODO: Initialize to an appropriate value
		//    target.Start();
		//    Assert.Inconclusive("A method that does not return a value cannot be verified.");
		//}

		///// <summary>
		/////A test for RPC
		/////</summary>
		//[TestMethod()]
		//public void RPCTest()
		//{
		//    string comPort = string.Empty; // TODO: Initialize to an appropriate value
		//    int baudRate = 0; // TODO: Initialize to an appropriate value
		//    MbedPortService target = new MbedPortService(comPort, baudRate); // TODO: Initialize to an appropriate value
		//    string function = string.Empty; // TODO: Initialize to an appropriate value
		//    int[] arguments = null; // TODO: Initialize to an appropriate value
		//    string expected = string.Empty; // TODO: Initialize to an appropriate value
		//    string actual;
		//    actual = target.RPC(function, arguments);
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		///// <summary>
		/////A test for SerialPort_ErrorReceived
		/////</summary>
		//[TestMethod()]
		//[DeploymentItem("KinectRCCar.CarController.dll")]
		//public void SerialPort_ErrorReceivedTest()
		//{
		//    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
		//    MbedPortService_Accessor target = new MbedPortService_Accessor(param0); // TODO: Initialize to an appropriate value
		//    object sender = null; // TODO: Initialize to an appropriate value
		//    SerialErrorReceivedEventArgs e = null; // TODO: Initialize to an appropriate value
		//    target.SerialPort_ErrorReceived(sender, e);
		//    Assert.Inconclusive("A method that does not return a value cannot be verified.");
		//}

		///// <summary>
		/////A test for Stop
		/////</summary>
		//[TestMethod()]
		//public void UninitializeTest()
		//{
		//    string comPort = string.Empty; // TODO: Initialize to an appropriate value
		//    int baudRate = 0; // TODO: Initialize to an appropriate value
		//    MbedPortService target = new MbedPortService(comPort, baudRate); // TODO: Initialize to an appropriate value
		//    target.Stop();
		//    Assert.Inconclusive("A method that does not return a value cannot be verified.");
		//}

		///// <summary>
		/////A test for IsRunning
		/////</summary>
		//[TestMethod()]
		//[DeploymentItem("KinectRCCar.CarController.dll")]
		//public void IsConnectedTest()
		//{
		//    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
		//    MbedPortService_Accessor target = new MbedPortService_Accessor(param0); // TODO: Initialize to an appropriate value
		//    bool expected = false; // TODO: Initialize to an appropriate value
		//    bool actual;
		//    target.IsRunning = expected;
		//    actual = target.IsRunning;
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//} 
		#endregion
	}
}
