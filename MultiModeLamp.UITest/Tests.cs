using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace MultiModeLamp.UITest
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;

		[SetUp]
		public void BeforeEachTest ()
		{
			// TODO: If the iOS app being tested is included in the solution then open
			// the Unit Tests window, right click Test Apps, select Add App Project
			// and select the app projects that should be tested.
			//
			// The iOS project should have the Xamarin.TestCloud.Agent NuGet package
			// installed. To start the Test Cloud Agent the following code should be
			// added to the FinishedLaunching method of the AppDelegate:
			//
			//    #if ENABLE_TEST_CLOUD
			//    Xamarin.Calabash.Start();
			//    #endif
			app = ConfigureApp
				.iOS
				.EnableLocalScreenshots()
			// TODO: Update this path to point to your iOS app and uncomment the
			// code if the app is not included in the solution.
			//.AppBundle ("../../../iOS/bin/iPhoneSimulator/Debug/MultiModeLamp.UITest.iOS.app")
				.StartApp ();
		}

		[Test]
		[Explicit]
		public void RunRepl()
		{
			app.Repl ();
		}

		[Test]
		public void ClickThroughAllScreens_Succeeds ()
		{
			// Show strobe light screen.
			app.Tap(x => x.Marked("Strobe"));
			// Info: app.Screenshot() returns a FileInfo object - so we could copy our screenshots somewhere! Sweet!
			var info = app.Screenshot("Tapped on Strobe toolbar");
			Console.WriteLine($"Screenshot info: {info}");

			// Show morse screen.
			app.Tap(x => x.Marked("Morse"));
			app.Screenshot("Tapped on view Morse toolbar");

			// Enter some text to morse and toggle switch.
			Func<AppQuery, AppQuery> txtMorseQuery = (AppQuery x) => x.Id("TextMorse");
			app.Tap(txtMorseQuery);
			app.Screenshot("Tapped morse text field");
			app.EnterText(txtMorseQuery, "Test");
			app.Screenshot("Entered 'Test' into text to morse");
			app.PressEnter();
			app.Tap(x => x.Id("ButtonToggleMorse"));
			app.Screenshot("Tapped on toggle morse button");

			// Switch to normal lamp.
			app.Tap(x => x.Marked("Normal Lamp"));
			app.Screenshot("Tapped on normal lamp toolbar");

			// Adjust brightness.
			Func<AppQuery, AppQuery> sliderQuery = (AppQuery x) => x.Id("SliderBrightness");
			app.SetSliderValue(sliderQuery, 0.3f);
			app.Screenshot("Setting brightness to 30%");
			app.SetSliderValue(sliderQuery, 0.8f);
			app.Screenshot("Setting brightness to 80%");

			// Click "about" button and wait for screen to appear.
			// Note: ID of the button is "ButtonDone", but querying it is not supported. Issue has been reported.
			Func<AppQuery, AppQuery> btnDoneQuery = (AppQuery x) => x.Marked("Done");
			app.Tap(x => x.Id("ButtonShowAbout"));
			app.WaitForElement(btnDoneQuery);
			app.Screenshot("Tapped on about button");

			// Close "about" screen.
			app.Tap(btnDoneQuery);
			app.Screenshot("Closed about dialog");
		}
	}
}

