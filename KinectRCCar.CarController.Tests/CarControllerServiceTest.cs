using KinectRCCar.CarController.Serial;
using KinectRCCar.Infrastructure.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace KinectRCCar.CarController.Tests
{
	[TestClass]
	public class CarControllerServiceTest
	{
		private Mock<MockMbedService> mockPortService;

		[TestInitialize]
		public void CarControllerServiceTestInitialize()
		{
			MockRepository repository = new MockRepository(MockBehavior.Loose);

			mockPortService = repository.Create<MockMbedService>();
		}

		[TestMethod]
		public void GetDirection_ReadCorrectValue_128()
		{
			double expected = 0.0;

			mockPortService.Setup(m => m.RPC("Read", 0)).Returns(128.ToString);

			
			ICarControllerService carControllerService = new CarControllerService(mockPortService.Object, new CarControllerSettings());

			double direction = carControllerService.GetDirection();
			Assert.AreEqual(expected, direction);
			mockPortService.VerifyAll();
		}

		[TestMethod]
		public void GetVelocity_ReadCorrectValue_128()
		{
			double expected = 0.0;

			mockPortService.Setup(m => m.RPC("Read", 1)).Returns(128.ToString);

			ICarControllerService carControllerService = new CarControllerService(mockPortService.Object, new CarControllerSettings());

			double velocity = carControllerService.GetVelocity();

			Assert.AreEqual(expected, velocity);
			mockPortService.VerifyAll();
		}

		[TestMethod]
		public void SetVelocity_GetVelocity()
		{
			double expectedVelocity = 1.0;

			mockPortService.Setup(m => m.RPC("Write", 1, It.IsInRange(0, 256, Range.Inclusive))).Returns(() => "144");
			mockPortService.Setup(m => m.RPC("Read", 1)).Returns(() => "144");
			

			ICarControllerService carControllerService = new CarControllerService(mockPortService.Object, new CarControllerSettings());

			carControllerService.SetVelocity(expectedVelocity);

			double velocity = carControllerService.GetVelocity();

			Assert.AreEqual(expectedVelocity, velocity);

		}

		[TestMethod]
		public void SetDirection_GetDirection()
		{
			double expectedDirection = 1.0;

			mockPortService.Setup(m => m.RPC("Write", 0, It.IsInRange(0, 256, Range.Inclusive))).Returns(() => "256");
			mockPortService.Setup(m => m.RPC("Read", 0)).Returns(() => "256");


			ICarControllerService carControllerService = new CarControllerService(mockPortService.Object, new CarControllerSettings());

			carControllerService.SetDirection(expectedDirection);

			double direction = carControllerService.GetDirection();

			Assert.AreEqual(expectedDirection, direction);
		}

		[TestMethod]
		public void Initialized_IsCalled_True()
		{
			mockPortService.Setup(m => m.Start());
			mockPortService.SetupGet(m => m.Status).Returns(MbedStatus.Connected);
			
			ICarControllerService carControllerService = new CarControllerService(mockPortService.Object, new CarControllerSettings());

			carControllerService.Start();

			mockPortService.VerifyAll();

		}

		[TestMethod]
		public void Uninitialized_IsCalled_True()
		{
			mockPortService.Setup(m => m.Start());
			mockPortService.Setup(m => m.RPC("Reset")).Returns(string.Empty);
			mockPortService.SetupGet(m => m.Status).Returns(MbedStatus.Connected);
			
			ICarControllerService carControllerService = new CarControllerService(mockPortService.Object, new CarControllerSettings());

			carControllerService.Start();
			Assert.AreEqual(CarControllerStatus.Connected, carControllerService.Status);

			
			carControllerService.Stop();
			

			Assert.AreEqual(CarControllerStatus.Disconnected, carControllerService.Status);

			mockPortService.VerifyAll();
		}
	}
}
