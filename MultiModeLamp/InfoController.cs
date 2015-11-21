using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MessageUI;

namespace MultiModeLamp
{
	partial class InfoController : UIViewController
	{
		public InfoController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.webView.LoadData (NSData.FromFile ("about.html"), "text/html", "UTF8", NSUrl.FromString ("."));
			this.webView.ShouldStartLoad = (webView, request, navigationType) => {
				if(request.Url.AbsoluteString.StartsWith("mailto:"))
				{
					string recipient = request.Url.AbsoluteString.Replace("mailto:", "");
					var mail = new MFMailComposeViewController();
					mail.SetToRecipients(new[] { recipient });
					mail.SetSubject("MultiMode LED");
					this.PresentViewController(mail, true, null);
					return false;
				}

				if(navigationType == UIWebViewNavigationType.LinkClicked)
				{
					UIApplication.SharedApplication.OpenUrl(request.Url);
					return false;
				}
				return true;
			};
		}
	}
}
