using Foundation;
using UIKit;
using Xamarin;

namespace MultiModeLamp
{
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Wire up Insights crash reporter.
			Insights.HasPendingCrashReport += (sender, isStartupCrash) => {
				if (isStartupCrash)
				{
					Insights.PurgePendingCrashReports ().Wait ();
				}
			};
			return true;
		}
	}
}