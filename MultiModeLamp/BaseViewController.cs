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

		protected AVCaptureDevice torchDevice;

		protected void ShowMessage (string msg)
		{
			var alert = UIAlertController.Create (string.Empty, msg, UIAlertControllerStyle.Alert);
			alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
			this.PresentViewController (alert, true, null);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.torchDevice = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video)?.FirstOrDefault (d => d.HasTorch);		

			// iOS turns torch turns off when entering background, so we set it to OFF here to keep the UI in sync.
			UIApplication.Notifications.ObserveDidEnterBackground (((sender, e) => this.SetTorchEnabled(false) ));
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (this.torchDevice == null)
			{
				this.ShowMessage ("No device with flash found. You're lost in the dark!");
			}
		}

		protected virtual void SetTorchEnabled (bool on)
		{
			if (this.torchDevice == null)
			{
				return;
			}
			NSError error;
			bool success = torchDevice.LockForConfiguration (out error);
			if (success && error == null)
			{
				torchDevice.TorchMode = on ? AVCaptureTorchMode.On : AVCaptureTorchMode.Off;
				torchDevice.UnlockForConfiguration ();
			}
			else
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
			if (this.torchDevice == null)
			{
				return;
			}

			// Setting brightness turns the lamp on - I don't want this behavior here. I want the brightness to act only if the lamp is on and use the current
			// brightness level as the initial value.
			if(this.torchDevice.TorchMode == AVCaptureTorchMode.Off)
			{
				return;
			}

			if(brightness < 0.1f)
			{
				brightness = 0.1f;
			}
			if(brightness > 1f)
			{
				brightness = 1f;
			}

			NSError error;

			// Set brightness.
			try
			{
				bool success = this.torchDevice.LockForConfiguration (out error);
				if (!success || error != null)
				{
					Insights.Report (new InvalidOperationException (error?.Description), Insights.Severity.Error);
				}
				success = this.torchDevice.SetTorchModeLevel (brightness, out error);

				if (!success || error != null)
				{
					Insights.Report (new InvalidOperationException (error?.Description), Insights.Severity.Error);
				}

				this.torchDevice.UnlockForConfiguration();
			}
			catch (Exception ex)
			{
				Insights.Report (ex, Insights.Severity.Error);
			}
		}
	}
}
