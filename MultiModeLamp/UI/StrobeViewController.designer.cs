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
	[Register ("StrobeViewController")]
	partial class StrobeViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnToggleStrobe { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblFlashesPerSecond { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider sliderFlashesPerSecond { get; set; }

		[Action ("HandleStrobeSliderChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleStrobeSliderChanged (UISlider sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnToggleStrobe != null) {
				btnToggleStrobe.Dispose ();
				btnToggleStrobe = null;
			}
			if (lblFlashesPerSecond != null) {
				lblFlashesPerSecond.Dispose ();
				lblFlashesPerSecond = null;
			}
			if (sliderFlashesPerSecond != null) {
				sliderFlashesPerSecond.Dispose ();
				sliderFlashesPerSecond = null;
			}
		}
	}
}
