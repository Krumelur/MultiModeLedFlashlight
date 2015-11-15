using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using AVFoundation;
using Refractored.Xam.Settings;
using System.Threading.Tasks;
using System.Threading;

namespace MultiModeLamp
{
	partial class StrobeViewController : BaseViewController
	{
		const string SETTING_STROBE_FPS = "StrobeSpeed";
		const int DEFAULT_FPS = 24;

		public StrobeViewController (IntPtr handle) : base (handle)
		{
		}

		CancellationTokenSource cts;
		TimeSpan delay;

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
		async Task Flash (CancellationToken token)
		{
			await Task.Run(async () => {
				while(!token.IsCancellationRequested)
				{
					this.SetTorchEnabled(true);
					await Task.Delay(this.delay);
					this.SetTorchEnabled(false);
					await Task.Delay(this.delay);
				}	
			});
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
				this.cts.Cancel();
			}
			else
			{
				this.cts = new CancellationTokenSource();
				this.UpdateUi(updateSlider: true);
				await this.Flash(this.cts.Token);
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
