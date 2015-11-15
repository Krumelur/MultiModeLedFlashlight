using UIKit;

namespace MultiModeLamp
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey);
			UIApplication.Main (args, null, typeof(AppDelegate));
		}
	}
}
