using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Threading;
using Refractored.Xam.Settings;
using System.Threading.Tasks;

namespace MultiModeLamp
{
	partial class MorseViewController : BaseViewController
	{
		const string SETTING_MORSE_CHARS_PER_MINUTE = "MorseSpeed";
		const float DEFAULT_CHARS_PER_MINUTE = 20;

		public MorseViewController (IntPtr handle) : base (handle)
		{
		}

		CancellationTokenSource cts;

		/// <summary>
		/// Length of a "."
		/// </summary>
		float LengthDit
		{
			get;
			set;
		}

		/// <summary>
		/// Length of a "-"
		/// </summary>
		float LengthDah
		{
			get
			{
				return this.LengthDit * 3;
			}
		}

		/// <summary>
		/// Length of break after a DIT or DAH
		/// </summary>
		/// <value>The length symbol break.</value>
		public float LengthSymbolBreak
		{
			get
			{
				return this.LengthDit;
			}
		}

		/// <summary>
		/// Length of break after a character
		/// </summary>
		public float LengthCharBreak
		{
			get
			{
				return this.LengthDit * 3;
			}
		}

		/// <summary>
		/// Length of break after a word
		/// </summary>
		public float LengthWordBreak
		{
			get
			{
				return this.LengthDit * 7;
			}
		}

		void UpdateUi(bool updateSlider)
		{
			if(updateSlider)
			{
				this.sliderMorseSpeed.SetValue(CrossSettings.Current.GetValueOrDefault(SETTING_MORSE_CHARS_PER_MINUTE, DEFAULT_CHARS_PER_MINUTE), true);
			}

			if(this.cts == null)
			{
				this.btnToggleMorse.SetBackgroundImage(UIImage.FromBundle("off.png"), UIControlState.Normal);
			}
			else
			{
				this.btnToggleMorse.SetBackgroundImage(UIImage.FromBundle("on.png"), UIControlState.Normal);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Slider values are appr "chars per minute" (1 means a length of DIT of 6000ms, 5 means a length of DIT of 1200ms, 100 means a length of DIT of 60ms, ...)
			// Length of DIT = 6000 / chars per minute.
			this.sliderMorseSpeed.MinValue = 5;
			this.sliderMorseSpeed.MaxValue = 100;

			this.LengthDit = 6000f / CrossSettings.Current.GetValueOrDefault(SETTING_MORSE_CHARS_PER_MINUTE, DEFAULT_CHARS_PER_MINUTE);

			this.btnToggleMorse.TouchUpInside += HandleToggleMorse;
			this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.View.EndEditing(true)));
		}

		async Task Morse (string text)
		{
			
		}

		async void HandleToggleMorse (object sender, EventArgs e)
		{
			if(this.cts != null)
			{
				this.cts.Cancel();
			}
			else
			{
				this.cts = new CancellationTokenSource();
				this.UpdateUi(updateSlider: true);
				await this.Morse(this.txtMorse.Text, this.cts.Token);
				this.cts = null;
				this.UpdateUi(updateSlider: true);
			}

		}


		/// <summary>
		/// Gets called if the slider to adjust the flash speed has been moved.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleMorseSpeedChanged (UISlider sender)
		{
			int speed = (int)Math.Round(sender.Value);

			// Remember last set value.
			CrossSettings.Current.AddOrUpdateValue(SETTING_MORSE_CHARS_PER_MINUTE, speed);

			// Update morse speed.
			this.LengthDit = 6000f / speed;

			this.UpdateUi(updateSlider: false);
		}
	}
}
