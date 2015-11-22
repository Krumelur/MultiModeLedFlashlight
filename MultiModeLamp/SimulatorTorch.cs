using UIKit;
using CoreGraphics;

namespace MultiModeLamp
{
	/// <summary>
	/// Simulator torch. Uses a small UIView which is placed at the top left corner of the screen.
	/// This is just there to allow development with the Simulator.
	/// </summary>
	public class SimulatorTorch : ITorch
	{
		UIView view;

		public SimulatorTorch ()
		{
			this.view = new UIView(new CGRect(0, 0, 1, 1));
			var parentView = UIApplication.SharedApplication.KeyWindow.RootViewController.View;
			parentView.Add(this.view);
			parentView.BringSubviewToFront(this.view);
		}

		public bool HasTorchInstalled
		{
			get
			{
				return true;
			}
		}

		public bool SetBrightness (float level)
		{
			if(!this.IsEnabled)
			{
				return true;
			}

			UIApplication.SharedApplication.InvokeOnMainThread(() => this.view.Alpha = level);
			return true;
		}

		public bool SetTorchEnabled (bool enabled)
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() => this.view.BackgroundColor = enabled ? UIColor.White : UIColor.Black);
			this.IsEnabled = enabled;
			return true;
		}

		public bool IsEnabled
		{
			get;
			private set;
		}
	}
}

