using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Threading;
using Refractored.Xam.Settings;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace MultiModeLamp
{
	partial class MorseViewController : BaseViewController
	{
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

		static bool IsValidMorseCharacter (char c)
		{
			string s = c.ToString ().ToUpperInvariant ();
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
		/// <value>The length symbol break.</value>
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

		void UpdateUi (bool updateSlider)
		{
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
		
			// Only need one method, use weak delegate instead of subclassing UITextFieldDelegate.
			this.txtMorse.WeakDelegate = this;
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

			// Return FALSE because we update the content ourselves (uppercasing everything).
			return false;
		}

		async Task Morse (string text, CancellationToken token)
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

				Debug.WriteLine ($"Current character: '{this.lblCurrentMorseChar.Text}' translates to '{currentMorseString}'");

				// Morse representation never starts or ends with blank. Trimming is just for sanity.
				currentMorseString = currentMorseString.Trim ();

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
		}

		async void HandleToggleMorse (object sender, EventArgs e)
		{
			if (this.cts != null)
			{
				this.cts.Cancel ();
			}
			else
			{
				this.cts = new CancellationTokenSource ();
				this.UpdateUi (updateSlider: true);
				this.txtMorse.Enabled = false;
				this.txtMorse.ResignFirstResponder ();
				try
				{
					while(this.switchRepeat.On)
					{
						await this.Morse (this.txtMorse.Text, this.cts.Token);
						await Task.Delay(LengthWordBreak, this.cts.Token);
					}
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
