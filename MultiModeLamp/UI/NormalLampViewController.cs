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
using Refractored.Xam.Settings;
using System.Threading.Tasks;
using Xamarin;

namespace MultiModeLamp
{
	/// <summary>
	/// Normal lamp view controller.
	/// </summary>
	partial class NormalLampViewController : BaseViewController
	{
		const string SETTING_BRIGHTNESS = "Brightness";
		const float DIM_ALPHA = 0.8f;

		public NormalLampViewController (IntPtr handle) : base (handle)
		{
		}

		[Action ("UnwindToNormalLampViewController:")]
		public void UnwindToNormalLampViewController (UIStoryboardSegue segue)
		{
			// Nothing in here...this is wired up in the iOS designer to exit from the modal info controller.
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			// Turn off lamp if we navigate somewhere else.
			this.SetTorchEnabled (false);
			this.UpdateUi (animate: false);

			base.PrepareForSegue (segue, sender);
		}

		protected override void AppEnteredBackground ()
		{
			base.AppEnteredBackground ();
			this.UpdateUi (false);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Add a view which dims the entire screen to reduce blinding if the lamp is on.
			this.dimView = new UIView {
				Frame = this.View.Bounds,
				Alpha = DIM_ALPHA,
				BackgroundColor = UIColor.Black,
				// This prevents the view from capturing touches.
				UserInteractionEnabled = false,
				Hidden = true
			};
			this.Add (dimView);
		}

		UIView dimView;

		public override void ViewDidAppear (bool animated)
		{
			Insights.Track ("Show normal lamp screen");
			base.ViewDidAppear (animated);
			this.UpdateUi (animate: false);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.SetTorchEnabled (false);
		}

		/// <summary>
		/// Helper to update the UI to the current torch settings.
		/// </summary>
		async Task UpdateUi (bool animate)
		{
			if (this.IsTorchAvailable)
			{
				this.sliderBrightness.Enabled = true;
				this.btnToggleLamp.Enabled = true;
				if (this.IsTorchEnabled)
				{
					this.btnToggleLamp.SetBackgroundImage (UIImage.FromBundle ("on.png"), UIControlState.Normal);
					this.btnToggleLamp.Superview.BringSubviewToFront (this.btnToggleLamp);

					// Dim screen if lamp is on.
					await this.SetDimViewEnabledAsync (enabled: true, animate: animate);
				}
				else
				{
					this.btnToggleLamp.SetBackgroundImage (UIImage.FromBundle ("off.png"), UIControlState.Normal);

					// Hide dim view view again.
					await this.SetDimViewEnabledAsync (enabled: false, animate: animate);
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
			}
		}

		protected override void SetTorchEnabled (bool on)
		{
			base.SetTorchEnabled (on);

			float brightness = CrossSettings.Current.GetValueOrDefault (SETTING_BRIGHTNESS, 1f);
			this.SetTorchBrightness (brightness);
		}

		/// <summary>
		/// Gets called if the button to toggle the lamp is pressed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		async partial void HandleToggleLamp (UIButton sender)
		{
			this.SetTorchEnabled (on: !this.IsTorchEnabled);
			Insights.Track ("Toggling lamp", "enabled", this.IsTorchEnabled ? "yes" : "no");
			await this.UpdateUi (animate: true);
		}

		/// <summary>
		/// Gets called if the slider to adjust the brightness has been moved.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleBrightnessChanged (UISlider sender)
		{
			CrossSettings.Current.AddOrUpdateValue (SETTING_BRIGHTNESS, sender.Value);
			this.SetTorchBrightness (sender.Value);

			this.UpdateBrightnesLevelText (sender.Value);
		}

		/// <summary>
		/// Gets called if the slide to change the brightness has been touched.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleSliderTouched (UISlider sender)
		{
			if (!this.IsTorchEnabled)
			{
				return;
			}

			// We want to hide the dim view while the lamp is on and the user adjusts the brightness.
			this.SetDimViewEnabledAsync (enabled: false, animate: true);
		}

		/// <summary>
		/// Gets called if touching the slider has ended.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleSliderTouchEnded (UISlider sender)
		{
			if (!this.IsTorchEnabled)
			{
				return;
			}

			// If the light is currently on, we want to show the dim view again.
			this.SetDimViewEnabledAsync (enabled: true, animate: true);
		}

		async Task SetDimViewEnabledAsync (bool enabled, bool animate)
		{
			if (enabled)
			{
				if (this.dimView.Hidden)
				{
					await UIView.AnimateAsync (
						duration: animate ? 0.2f : 0f,
						animation: () => {
							this.dimView.Alpha = 0f;
							this.dimView.Hidden = false;
							this.dimView.Alpha = DIM_ALPHA;
						});
				}
			}
			else
			{
				if (!this.dimView.Hidden)
				{
					await UIView.AnimateAsync (
						duration: animate ? 0.2f : 0f,
						animation: () => this.dimView.Alpha = 0f);
					this.dimView.Hidden = true;
				}
			}
		}

		/// <summary>
		/// Helper to update the label which is showing the current brightness in percent.
		/// </summary>
		/// <param name="level">Level.</param>
		void UpdateBrightnesLevelText (float level)
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
