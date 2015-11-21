// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MultiModeLamp
{
	[Register ("MorseViewController")]
	partial class MorseViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnToggleMorse { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblCurrentMorseChar { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider sliderMorseSpeed { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch switchRepeat { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtMorse { get; set; }

		[Action ("HandleMorseSpeedChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleMorseSpeedChanged (UISlider sender);

		[Action ("HandleRepeatChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleRepeatChanged (UISwitch sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnToggleMorse != null) {
				btnToggleMorse.Dispose ();
				btnToggleMorse = null;
			}
			if (lblCurrentMorseChar != null) {
				lblCurrentMorseChar.Dispose ();
				lblCurrentMorseChar = null;
			}
			if (sliderMorseSpeed != null) {
				sliderMorseSpeed.Dispose ();
				sliderMorseSpeed = null;
			}
			if (switchRepeat != null) {
				switchRepeat.Dispose ();
				switchRepeat = null;
			}
			if (txtMorse != null) {
				txtMorse.Dispose ();
				txtMorse = null;
			}
		}
	}
}
