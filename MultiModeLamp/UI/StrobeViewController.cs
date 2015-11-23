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

using System;
using UIKit;
using Refractored.Xam.Settings;
using System.Threading.Tasks;
using System.Threading;
using Xamarin;

namespace MultiModeLamp
{
	/// <summary>
	/// Strobe view controller.
	/// </summary>
	partial class StrobeViewController : BaseViewController
	{
		const string SETTING_STROBE_FPS = "StrobeSpeed";
		const int DEFAULT_FPS = 24;

		public StrobeViewController (IntPtr handle) : base (handle)
		{
		}

		CancellationTokenSource cts;
		TimeSpan delay;

		protected override void AppEnteredBackground ()
		{
			this.cts?.Cancel ();
			base.AppEnteredBackground ();
			this.UpdateUi (false);
		}

		/// <summary>
		/// Updates the user interface.
		/// </summary>
		/// <param name="updateSlider">If set to <c>true</c> update slider.</param>
		void UpdateUi(bool updateSlider)
		{
			if(updateSlider)
			{
				this.sliderFlashesPerSecond.SetValue(CrossSettings.Current.GetValueOrDefault(SETTING_STROBE_FPS, DEFAULT_FPS), true);
			}

			if(this.cts == null)
			{
				this.btnToggleStrobe.SetBackgroundImage(UIImage.FromBundle("off.png"), UIControlState.Normal);
			}
			else
			{
				this.btnToggleStrobe.SetBackgroundImage(UIImage.FromBundle("on.png"), UIControlState.Normal);
			}

			this.lblFlashesPerSecond.Text = ((int)Math.Round(this.sliderFlashesPerSecond.Value)).ToString("D");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.sliderFlashesPerSecond.MinValue = 1;
			this.sliderFlashesPerSecond.MaxValue = 60;

			this.delay = TimeSpan.FromMilliseconds(1f / CrossSettings.Current.GetValueOrDefault(SETTING_STROBE_FPS, DEFAULT_FPS) * 1000f / 2f);

			this.btnToggleStrobe.TouchUpInside += this.HandleToggleStrobe;
		}

		public override void ViewDidAppear (bool animated)
		{
			Insights.Track ("Show strobe screen");
			base.ViewDidAppear (animated);
			this.UpdateUi(updateSlider: true);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.cts?.Cancel();
		}

		/// <summary>
		/// Flashes the lamp.
		/// </summary>
		/// <param name="token">Token.</param>
		async Task FlashAsync (CancellationToken token)
		{
			try
			{
			await Task.Run(async () => {
				while(!token.IsCancellationRequested)
				{
					this.SetTorchEnabled(true);
					await Task.Delay(this.delay, token);
					this.SetTorchEnabled(false);
					await Task.Delay(this.delay, token);
				}	
			}, token);
			}
			catch(OperationCanceledException)
			{
				// Expected when passing a token to Task.Delay().
			}
			this.SetTorchEnabled(false);
		}

		/// <summary>
		/// Gets called if the button to toggle the strobe light was tapped.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		async void HandleToggleStrobe (object sender, EventArgs args)
		{
			if(this.cts != null)
			{
				Insights.Track ("Stopping strobe");
				this.cts.Cancel();
			}
			else
			{
				Insights.Track ("Starting strobe");
				this.cts = new CancellationTokenSource();
				this.UpdateUi(updateSlider: true);
				await this.FlashAsync(this.cts.Token);
				this.cts = null;
				this.UpdateUi(updateSlider: true);
			}
		}

		/// <summary>
		/// Gets called if the slider to adjust the flash speed has been moved.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleStrobeSliderChanged (UISlider sender)
		{
			int fps = (int)Math.Round(sender.Value);

			// Remember last set value.
			CrossSettings.Current.AddOrUpdateValue(SETTING_STROBE_FPS, fps);

			// Update delay for flashing.
			this.delay = TimeSpan.FromMilliseconds(1f / (float)fps * 1000f / 2f);

			this.UpdateUi(updateSlider: false);
		}
	}
}
