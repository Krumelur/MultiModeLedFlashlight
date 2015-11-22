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
