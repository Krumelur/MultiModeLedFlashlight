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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Xamarin;
using Plugin.Settings;

namespace MultiModeLamp
{
	/// <summary>
	/// Allows to morse message using the torch.
	/// </summary>
	partial class MorseViewController : BaseViewController
	{
		const string SETTING_LAST_MESSAGE = "LastMorseMessage";
		const string SETTING_MORSE_CHARS_PER_MINUTE = "MorseSpeed";
		const int DEFAULT_CHARS_PER_MINUTE = 20;
		const string SETTING_REPEAT_MORSE_MESSAGE = "RepeatMorse";

		static Dictionary<string, string> morseCodes = new Dictionary<string, string> {
			["A" ] = ". -",
			["B" ] = "- . . .",
			["C" ] = "- . - .",
			["D" ] = "- . .",
			["E" ] = ".",
			["F" ] =". . - .",
			["G" ] = "- - .",
			["H" ] = ". . . .",
			["I" ] =". .",
			["J" ] = ". - - -",
			["K" ] = "- . -",
			["L" ] = ". - . .",
			["M" ] = "- -",
			["N" ] = "- .",
			["O" ] = "- - -",
			["P" ] = ". - - .",
			["Q" ] = "- - . -",
			["R" ] = ". - .",
			["S" ] = ". . .",
			["T" ] = "-",
			["U" ] = ". . -",
			["V" ] = ". . . -",
			["W" ] = ". - -",
			["X" ] = "- . . -",
			["Y" ] = "- . - -",
			["Z" ] = "- - . .",
			["0" ] = "- - - - -",
			["1" ] = ". - - - -",
			["2" ] = ". . - - -",
			["3" ] = ". . . - -",
			["4" ] = ". . . . -",
			["5" ] = ". . . . .",
			["6" ] = "- . . . .",
			["7" ] = "- - . . .",
			["8" ] = "- - - . .",
			["9" ] = "- - - - .",
			["À" ] = ". - - . -",
			["Å" ] = ". - - . -",
			["Ä" ] = ". - . -",
			["È" ] = ". - . . -",
			["É" ] = ". . - . .",
			["Ö" ] = "- - - .",
			["Ü" ] = ". . - -",
			["ß" ] = ". . . - - . .",
			["Ñ" ] = "- - . - -",
			[".	" ] = ". - . - . -",
			[",	" ] = "- - . . - -",
			[":	" ] = "- - - . . .",
			[";	" ] = "- . - . - .",
			["?" ] = ". . - - . .",
			["-" ] = "- . . . . -",
			["_" ] = ". . - - . -",
			["(" ] = "- . - - .",
			[")" ] = "- . - - . -",
			["'	." ] = "- - - - .",
			["=" ] = "- . . . -",
			["+" ] = ". - . - .",
			["/" ] = "- . . - .",
			["@" ] = ". - - . - ."
		};

		/// <summary>
		/// Checks if a given character can be translated into a morse code.
		/// </summary>
		/// <returns><c>true</c> if is valid morse character the specified c; otherwise, <c>false</c>.</returns>
		/// <param name="c">C.</param>
		static bool IsValidMorseCharacter (char c)
		{
			string s = c.ToString ().ToUpperInvariant ();
			// It's either a blank or a character from the dictionary.
			return s == " " || morseCodes.ContainsKey (s);
		}


		public MorseViewController (IntPtr handle) : base (handle)
		{
		}

		CancellationTokenSource cts;

		/// <summary>
		/// Length of a "."
		/// </summary>
		static int LengthDit
		{
			get;
			set;
		}

		/// <summary>
		/// Length of a "-"
		/// </summary>
		static int LengthDah
		{
			get {
				return LengthDit * 3;
			}
		}

		/// <summary>
		/// Length of break after a DIT or DAH
		/// </summary>
		static int LengthSymbolBreak
		{
			get {
				return LengthDit;
			}
		}

		/// <summary>
		/// Length of break after a character
		/// </summary>
		static int LengthCharBreak
		{
			get {
				return LengthDit * 3;
			}
		}

		/// <summary>
		/// Length of break after a word
		/// </summary>
		static int LengthWordBreak
		{
			get {
				return LengthDit * 7;
			}
		}

		protected override void AppEnteredBackground ()
		{
			this.cts?.Cancel ();
			base.AppEnteredBackground ();
			this.UpdateUi (false);
		}

		/// <summary>
		/// Updates the user interface.
		/// </summary>
		/// <param name="updateSlider">If set to <c>true</c> also update slider.</param>
		void UpdateUi (bool updateSlider)
		{
			this.txtMorse.Text = CrossSettings.Current.GetValueOrDefault (SETTING_LAST_MESSAGE, string.Empty);

			if (updateSlider)
			{
				this.sliderMorseSpeed.SetValue (CrossSettings.Current.GetValueOrDefault (SETTING_MORSE_CHARS_PER_MINUTE, DEFAULT_CHARS_PER_MINUTE), true);
			}

			this.switchRepeat.On = CrossSettings.Current.GetValueOrDefault (SETTING_REPEAT_MORSE_MESSAGE, true);

			if (this.cts == null)
			{
				this.btnToggleMorse.SetBackgroundImage (UIImage.FromBundle ("off.png"), UIControlState.Normal);
			}
			else
			{
				this.btnToggleMorse.SetBackgroundImage (UIImage.FromBundle ("on.png"), UIControlState.Normal);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			Insights.Track ("Show morse screen");
			base.ViewDidAppear (animated);
			this.UpdateUi (true);
		}

		public override void ViewWillDisappear (bool animated)
		{
			this.cts?.Cancel();
			this.SetTorchEnabled(false);
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Slider values are appr "chars per minute" (1 means a length of DIT of 6000ms, 5 means a length of DIT of 1200ms, 100 means a length of DIT of 60ms, ...)
			// Length of DIT = 6000 / chars per minute.
			this.sliderMorseSpeed.MinValue = 5;
			this.sliderMorseSpeed.MaxValue = 100;

			LengthDit = (int)(6000f / CrossSettings.Current.GetValueOrDefault (SETTING_MORSE_CHARS_PER_MINUTE, DEFAULT_CHARS_PER_MINUTE));

			this.btnToggleMorse.TouchUpInside += HandleToggleMorse;
			this.View.AddGestureRecognizer (new UITapGestureRecognizer (() => this.View.EndEditing (true)));
		
			// Don't like to to implement my own UITextFieldDelegate subclass...
			this.txtMorse.WeakDelegate = this;
		}

		[Preserve]
		[Export ("textFieldShouldReturn:")]
		bool ShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder ();

			return true;
		}

		[Preserve]
		[Export ("textField:shouldChangeCharactersInRange:replacementString:")]
		bool ShouldChangeCharacters (UITextField textField, NSRange range, string replacementString)
		{
			bool hasInvalidChars = replacementString.ToCharArray ().Any (c => !IsValidMorseCharacter (c));
		
			if (hasInvalidChars)
			{
				UIView.Animate (0.2f, 0f, UIViewAnimationOptions.Autoreverse,
					() => this.txtMorse.BackgroundColor = UIColor.Red,
					() => this.txtMorse.BackgroundColor = UIColor.White);

				return false;
			}

			// Morse only has uppercase characters.
			replacementString = replacementString.ToUpperInvariant ();
			var nativeString = new NSString (this.txtMorse.Text);
			this.txtMorse.Text = nativeString.Replace (range, new NSString (replacementString));

			// Remember last message.
			CrossSettings.Current.AddOrUpdateValue (SETTING_LAST_MESSAGE, this.txtMorse.Text);

			// Return FALSE because we update the content ourselves (uppercasing everything).
			return false;
		}

		/// <summary>
		/// Morse the specified text.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="token">Token.</param>
		async Task MorseAsync (string text, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace (text))
			{
				return;
			}

			// Blanks in beginning or end don't make sense.
			text = text.Trim ();

			Debug.WriteLine ($"Morsing '{text}'");

			// Loop the text to morse character by character.
			for (int textIndex = 0; textIndex < text.Length; textIndex++)
			{
				string currentChar = text [textIndex].ToString ();
				string currentMorseString;

				// Update UI.
				this.lblCurrentMorseChar.Text = currentChar.ToUpperInvariant ();

				// A space is just a delay.
				if (currentChar == " ")
				{
					Debug.WriteLine ("(WORD BREAK) ");
					await Task.Delay (LengthWordBreak, token);
					continue;
				}

				// Get the morse representation of the current character.
				if (!morseCodes.TryGetValue (currentChar, out currentMorseString))
				{
					// Skip unknown characters.
					continue;
				}

				// Morse representation never starts or ends with blank. Trimming is just for sanity.
				currentMorseString = currentMorseString.Trim ();

				Debug.WriteLine ($"Current character: '{this.lblCurrentMorseChar.Text}' translates to '{currentMorseString}'");

				this.lblCurrentClearTextChar.Text = currentChar;

				// Loop the morse representation characters and turn light on or of accordingly.
				for (int morseIndex = 0; morseIndex < currentMorseString.Length; morseIndex++)
				{
					token.ThrowIfCancellationRequested ();

					char morseChar = currentMorseString [morseIndex];

					switch (morseChar)
					{
					// DAH
						case '-':
							Debug.WriteLine ("DAH ");
							this.lblCurrentMorseChar.TextColor = UIColor.LightTextColor;
							this.SetTorchEnabled (true);
							await Task.Delay (LengthDah, token);
							this.lblCurrentMorseChar.TextColor = UIColor.Red;
							this.SetTorchEnabled (false);
							break;
						
					// DIT
						case '.':
							Debug.WriteLine ("DIT ");
							this.lblCurrentMorseChar.TextColor = UIColor.LightTextColor;
							this.SetTorchEnabled (true);
							await Task.Delay (LengthDit, token);
							this.lblCurrentMorseChar.TextColor = UIColor.Red;
							this.SetTorchEnabled (false);
							break;

					// after each morse symbol there is a short break unless it is the last symbol
						default:
							if (morseIndex < currentMorseString.Length - 1)
							{
								Debug.WriteLine ("(SYMBOL BREAK) ");
								await Task.Delay (LengthSymbolBreak, token);
							}
							break;
					}
				}

				// After each morse character, there is a character break, unless it is the last character in the text.
				if (textIndex < text.Length - 1 && text[textIndex + 1] != ' ')
				{
					Debug.WriteLine ("(CHAR BREAK) ");
					await Task.Delay (LengthCharBreak, token);
				}
			}	

			this.lblCurrentClearTextChar.Text = string.Empty;
		}

		/// <summary>
		/// Gets called if the button to switch on the torch has been clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void HandleToggleMorse (object sender, EventArgs e)
		{
			if (this.cts != null)
			{
				Insights.Track ("Stopping morsing");
				// Cancel any currently ongoing morsing.
				this.cts.Cancel ();
			}
			else
			{
				if (string.IsNullOrWhiteSpace (this.txtMorse.Text))
				{
					return;
				}

				Insights.Track ("Starting morsing");

				// Morse the message.
				this.cts = new CancellationTokenSource ();
				this.UpdateUi (updateSlider: true);
				this.txtMorse.Enabled = false;
				this.txtMorse.ResignFirstResponder ();
				try
				{
					do
					{
						await this.MorseAsync (this.txtMorse.Text, this.cts.Token);
						await Task.Delay(LengthWordBreak, this.cts.Token);
					}
					while(this.switchRepeat.On);
				}
				catch (OperationCanceledException)
				{
					// Expected.
				}
				this.txtMorse.Enabled = true;
				this.cts = null;
				this.UpdateUi (updateSlider: true);
			}
		}


		/// <summary>
		/// Gets called if the slider to adjust the flash speed has been moved.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void HandleMorseSpeedChanged (UISlider sender)
		{
			int speed = (int)Math.Round (sender.Value);

			// Remember last set value.
			CrossSettings.Current.AddOrUpdateValue (SETTING_MORSE_CHARS_PER_MINUTE, speed);

			// Update morse speed.
			LengthDit = (int)(6000f / speed);

			this.UpdateUi (updateSlider: false);
		}

		partial void HandleRepeatChanged (UISwitch sender)
		{
			CrossSettings.Current.AddOrUpdateValue(SETTING_REPEAT_MORSE_MESSAGE, sender.On);
		}
	}
}
