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
		const string SETTING_DIM_SCREEN = "DimScreen";
		const string SETTING_BRIGHTNESS = "Brightness";

		public NormalLampViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.UpdateUi();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.SetTorchEnabled(false);
		}

		/// <summary>
		/// Helper to update the UI to the current torch settings.
		/// </summary>
		void UpdateUi ()
		{
			if (this.torchDevice != null)
			{
				this.sliderBrightness.Enabled = true;
				this.btnToggleLamp.Enabled = true;
				if (this.torchDevice.TorchMode == AVCaptureTorchMode.On)
				{
					this.btnToggleLamp.SetBackgroundImage(UIImage.FromBundle("on.png"), UIControlState.Normal);
					this.imgViewLamp.Image = UIImage.FromBundle ("lamp.png");
				}
				else
				{
					this.btnToggleLamp.SetBackgroundImage(UIImage.FromBundle("off.png"), UIControlState.Normal);
					this.imgViewLamp.Image = UIImage.FromBundle ("lamp_off.png");
				}

				float brightness = CrossSettings.Current.GetValueOrDefault(SETTING_BRIGHTNESS, 1f);
				this.sliderBrightness.SetValue (brightness, true);
			}
			else
			{
				this.sliderBrightness.Enabled = false;
				this.btnToggleLamp.Enabled = false;
				this.imgViewLamp.Image = UIImage.FromBundle ("lamp_off.png");
			}

			this.switchDimScreen.On = CrossSettings.Current.GetValueOrDefault (SETTING_DIM_SCREEN, true);

			// If the switch to dim the screen is on, fade out the background image to reduce brightness of UI.
			if (this.switchDimScreen.On)
			{
				UIView.Animate (0.5f, () => this.imgViewLamp.Alpha = 0.26f);
			}
			else
			{
				UIView.Animate (0.5f, () => this.imgViewLamp.Alpha = 1f);
			}
		}

		/// <summary>
		/// Turns the torch on or off.
		/// </summary>
		/// <param name="on">If set to <c>true</c> on.</param>
		protected override void SetTorchEnabled (bool on)
		{
			base.SetTorchEnabled (on);

			float brightness = CrossSettings.Current.GetValueOrDefault(SETTING_BRIGHTNESS, 1f);
			this.SetTorchBrightness(brightness);

			this.UpdateUi ();
		}

		/// <summary>
		/// Gets called if the button to switch lamp on or off has been tapped.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleToggleLamp (UIButton sender)
		{
			if (this.torchDevice == null)
			{
				return;
			}

			SetTorchEnabled (this.torchDevice.TorchMode == AVCaptureTorchMode.Off ? true : false);
		}

		partial void HandleDimScreenChanged (UISwitch sender)
		{
			CrossSettings.Current.AddOrUpdateValue (SETTING_DIM_SCREEN, sender.On);
			this.UpdateUi();
		}

		/// <summary>
		/// Gets called if the slider to adjust the brightness has been moved.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleBrightnessChanged (UISlider sender)
		{
			CrossSettings.Current.AddOrUpdateValue (SETTING_BRIGHTNESS, sender.Value);
			this.SetTorchBrightness(sender.Value);
		}
	}
}
