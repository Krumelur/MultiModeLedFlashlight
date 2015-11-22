using System;
using UIKit;
using Foundation;

namespace MultiModeLamp
{
	/// <summary>
	/// Base class for all view controllers in this app. Gives direct access to the torch through ITorch interface.
	/// </summary>
	public class BaseViewController : UIViewController
	{
		// SImulator doesn't have a flash. We use a small view as torch replacement.
		static Lazy<ITorch> torch = new Lazy<ITorch>(() => {
			#if SIMULATOR
			return new SimulatorTorch();
			#else
			return new FlashTorch();
			#endif
		}
		);

		/// <summary>
		/// Gets access to the torch (flash).
		/// </summary>
		protected  ITorch Torch
		{
			get
			{
				return BaseViewController.torch.Value;
			}
		}

		public BaseViewController (IntPtr handle) : base (handle)
		{
		}

		/// <summary>
		/// Shows a message dialog.
		/// </summary>
		/// <param name="msg">Message.</param>
		protected void ShowMessage (string msg)
		{
			var alert = UIAlertController.Create (string.Empty, msg, UIAlertControllerStyle.Alert);
			alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
			this.PresentViewController (alert, true, null);
		}

		/// <summary>
		/// Gets called if the app is about to enter background mode.
		/// </summary>
		protected virtual void AppEnteredBackground()
		{
			this.SetTorchEnabled (false);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// iOS turns torch turns off when entering background, so we set it to OFF here to keep the UI in sync.
			this.backgroundObserver = UIApplication.Notifications.ObserveDidEnterBackground (((sender, e) => this.AppEnteredBackground()));
		}

		NSObject backgroundObserver;

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.backgroundObserver.Dispose ();
			this.backgroundObserver = null;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (!this.IsTorchAvailable)
			{
				this.ShowMessage ("No device with flash found. You're lost in the dark!");
			}
		}

		/// <summary>
		/// Sets the torch enabled.
		/// </summary>
		/// <param name="on">If set to <c>true</c> on.</param>
		protected virtual void SetTorchEnabled (bool on)
		{
			if(!this.Torch.SetTorchEnabled(on))
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
			this.Torch.SetBrightness(brightness);
		}

		/// <summary>
		/// Gets a value indicating whether the torch is enabled.
		/// </summary>
		protected bool IsTorchEnabled
		{
			get
			{ 
				return this.Torch.IsEnabled;
			}
		}

		/// <summary>
		/// Gets a value indicating whether a torch is available.
		/// </summary>
		protected bool IsTorchAvailable
		{
			get
			{
				return this.Torch.HasTorchInstalled;
			}
		}
	}
}
