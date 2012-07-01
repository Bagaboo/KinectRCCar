Kinect/RC Car

This is the code for my Master Project. It allows you to control a remote control car with the Microsoft Kinect.

The program was written in C# using VS2010 Ultimate. You should be able to use any other version of VS2010, only the Modeling project probably won't load. I haven't tried loading this in Visual C# Express, but I don't see why it won't work.

In order to use the software, you need to have a remote control car that the program is able to interface to. To do this you must use a hardware solution that interfaces between the remote control car's transmitter and the computer. You must then create a plugin DLL the Kinect/RC Car program then uses to communicate with your hardware solution. You can look at my solution in the KinectRC.CarControllerService project which communicates with an mbed microcontroller prototype board which in turn controls an MCP4261. 

To make your own plugin DLL:

1. Create a class library project
2. Reference the KinectRCCar.Infrastructure project
3. Reference the Unity IOC Library and the PRISM library from NuGet or download from their respective websites.
4. Create a class that implements the ICarControllerService interface.
5. Write the code in this class that communicates with your hardware solution.
6. Create a Module class that inherits from the PRISM IModule interface.
7. Register your CarControllerService in the Initilization method.
8. Add the following attribute above the class declaration in your Module class: [Module(ModuleName = "CarControllerModule")]
9. Build the project then place the DLL in the Modules folder in the KinectRCCar build folder.
10. Remove the current CarControllerService DLL from the Modules folder.
11. Run the program.

Additional information on how the program was written is in the Project Report (ProjectReport.pdf. I may add additional code comments to explain more at a later time however this is a low priority. In addition, I won't be doing any more major work on this project as I have other new projects I wish to pursue. But I am willing to look at any Pull Requests you have. There is additional work that could be done such as making the UI nicer, converting the ControlLogic.cs class to use an appropriate design pattern (possibly Template Method), unit tests, and finding a better wireless camera solution.

You may contact me at devin@devinhoward.com if you have any questions.

Links:

Video of project running: http://youtu.be/3dLdfg4-niQ