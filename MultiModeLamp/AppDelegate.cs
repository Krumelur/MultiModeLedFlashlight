/*
* Copyright 2015 c-sharX (René Ruppert)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* - The Software code may not be used to create a clone of the app for commercial use.
* - For any commercial use, the Software must be modified so that it is clearly distinguishable from the original version.
* - For any commercial use the "c-sharX" logo must be removed or replaced with another logo.
* - For any commercial use the "about.html" file must be removed or replaced with different content.
*/

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

			// This is used to enable remote controlling the app, both for local UI Tests and also for use in TestCloud.
			// Never include this into a release build!
			#if ENABLE_TEST_CLOUD
			Calabash.Start();
			#endif
			return true;
		}
	}
}