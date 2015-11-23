# MultiMode LED

MultiMode LED was first built by me in 2010 when I started developing for the iPhone. 
The original version used ObjectiveC and evetually found its way into the Apple App Store.
Five years later it is time to convert the puppy over to C# - using Xamarin.iOS - and also to make the source available.

The app features:

* Usage as a flashlight (adjustable brightness)
* Strobe (adjustable speed)
* Morsing (adjustable speed)

It was built using Xamarin.iOS 9.2.1 and the Apple tools shipping with Xcode 7.1.
Minimum required iOS version is 8.4. The app uses Storyboards and adjusts to iPhone 4, 5, 6 and 6+ screens.

The source demonstrates:

* Access the flash/torch
* Make use of async/await and support cancellation
* Create UI in Xamarin.iOS designer (Storyboards)
* Integrate Xamarin.Insights
* Use Xamarin UI Test

To really use the app, make sure to run it on a physical device; the Simulator does not have a flash!

This flashlight app is commercially available on the App Store: https://itunes.apple.com/us/app/multimode-led-taschenlampe/id393966530

Please refer to the LICENSE before you use any portions of this code.
