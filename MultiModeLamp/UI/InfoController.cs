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
using System;
using UIKit;
using MessageUI;
using Xamarin;

namespace MultiModeLamp
{
	/// <summary>
	/// Shows information about the app.
	/// </summary>
	partial class InfoController : UIViewController
	{
		public InfoController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// HTML is perfect to display this kind of content.
			this.webView.LoadData (NSData.FromFile ("about.html"), "text/html", "UTF8", NSUrl.FromString ("."));

			//  We want to open links in the real Safari and mailto links should also be handled correctly.
			this.webView.ShouldStartLoad = (webView, request, navigationType) => {

				// If a "mailto:" link is clicked, open a mail composition sheet.
				if(request.Url.AbsoluteString.StartsWith("mailto:"))
				{
					string recipient = request.Url.AbsoluteString.Replace("mailto:", "");
					var mail = new MFMailComposeViewController();
					mail.SetToRecipients(new[] { recipient });
					mail.SetSubject("MultiMode LED");
					mail.Finished += (sender, e) => ((MFMailComposeViewController)sender).DismissViewController(true, null);
					this.PresentViewController(mail, true, null);
					return false;
				}

				// For other links, open Safari.
				if(navigationType == UIWebViewNavigationType.LinkClicked)
				{
					UIApplication.SharedApplication.OpenUrl(request.Url);
					return false;
				}
				return true;
			};
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			Insights.Track ("Show about screen");
		}
	}
}
