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
	partial class NormalLampViewController : BaseViewController
	{
		const string SETTING_BRIGHTNESS = "Brightness";

		public NormalLampViewController (IntPtr handle) : base (handle)
		{
		}

		[Action ("UnwindToNormalLampViewController:")]
		public void UnwindToNormalLampViewController (UIStoryboardSegue segue)
		{
			// Nothing in here...this is wired up in the iOS designer to exit from the modal info controller.
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.UpdateUi ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.SetTorchEnabled (false);
		}

		/// <summary>
		/// Helper to update the UI to the current torch settings.
		/// </summary>
		void UpdateUi ()
		{
			if (AppDelegate.Torch.HasTorchInstalled)
			{
				this.sliderBrightness.Enabled = true;
				this.btnToggleLamp.Enabled = true;
				if (AppDelegate.Torch.IsEnabled)
				{
					this.btnToggleLamp.SetBackgroundImage (UIImage.FromBundle ("on.png"), UIControlState.Normal);
					//this.imgViewLamp.Image = UIImage.FromBundle ("lamp.png");
				}
				else
				{
					this.btnToggleLamp.SetBackgroundImage (UIImage.FromBundle ("off.png"), UIControlState.Normal);
					//this.imgViewLamp.Image = UIImage.FromBundle ("lamp_off.png");
				}

				float brightness = CrossSettings.Current.GetValueOrDefault (SETTING_BRIGHTNESS, 1f);
				this.sliderBrightness.SetValue (brightness, true);
				this.UpdateBrightnesLevelText (brightness);
			}
			else
			{
				this.sliderBrightness.Enabled = false;
				this.btnToggleLamp.Enabled = false;
				this.UpdateBrightnesLevelText (0f);
				//this.imgViewLamp.Image = UIImage.FromBundle ("lamp_off.png");
			}
		}

		/// <summary>
		/// Turns the torch on or off.
		/// </summary>
		/// <param name="on">If set to <c>true</c> on.</param>
		protected override void SetTorchEnabled (bool on)
		{
			base.SetTorchEnabled (on);

			float brightness = CrossSettings.Current.GetValueOrDefault (SETTING_BRIGHTNESS, 1f);
			this.SetTorchBrightness (brightness);

			this.UpdateUi ();
		}

		/// <summary>
		/// Gets called if the button to switch lamp on or off has been tapped.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleToggleLamp (UIButton sender)
		{
			AppDelegate.Torch.SetTorchEnabled (!AppDelegate.Torch.IsEnabled);
			this.UpdateUi ();
		}


		/// <summary>
		/// Gets called if the slider to adjust the brightness has been moved.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleBrightnessChanged (UISlider sender)
		{
			CrossSettings.Current.AddOrUpdateValue (SETTING_BRIGHTNESS, sender.Value);
			this.SetTorchBrightness (sender.Value);

			this.UpdateBrightnesLevelText(sender.Value);
		}

		void UpdateBrightnesLevelText(float level)
		{
			int roundedLevel = ((int)(Math.Round (level * 10) * 10));
			if (roundedLevel < 10)
			{
				roundedLevel = 10;
			}
			this.lblLevel.Text = $"{roundedLevel}%";
		}
	}
}
