using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using AVFoundation;
using System.Linq;
using Refractored.Xam.Settings;
using Xamarin;

namespace MultiModeLamp
{
	partial class BaseViewController : UIViewController
	{
		public BaseViewController (IntPtr handle) : base (handle)
		{
		}

		protected void ShowMessage (string msg)
		{
			var alert = UIAlertController.Create (string.Empty, msg, UIAlertControllerStyle.Alert);
			alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
			this.PresentViewController (alert, true, null);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();



			// iOS turns torch turns off when entering background, so we set it to OFF here to keep the UI in sync.
			UIApplication.Notifications.ObserveDidEnterBackground (((sender, e) => this.SetTorchEnabled(false) ));
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (!AppDelegate.Torch.HasTorchInstalled)
			{
				this.ShowMessage ("No device with flash found. You're lost in the dark!");
			}
		}

		protected virtual void SetTorchEnabled (bool on)
		{
			if(!AppDelegate.Torch.SetTorchEnabled(on))
			{
				this.ShowMessage ("Failed to light the torch. You're lost in the dark!");
			}
		}

		/// <summary>
		/// Changes the brightness.
		/// </summary>
		/// <param name="brightness">Brightness (0 .. 1).</param>
		protected virtual void SetTorchBrightness(float brightness)
		{
			AppDelegate.Torch.SetBrightness(brightness);
		}
	}
}
