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
	[Register ("NormalLampViewController")]
	partial class NormalLampViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnInfo { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnToggleLamp { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgViewLamp { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblLevel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider sliderBrightness { get; set; }

		[Action ("HandleBrightnessChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleBrightnessChanged (UISlider sender);

		[Action ("HandleSliderTouched:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleSliderTouched (UISlider sender);

		[Action ("HandleSliderTouchEnded:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleSliderTouchEnded (UISlider sender);

		[Action ("HandleToggleLamp:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleToggleLamp (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnInfo != null) {
				btnInfo.Dispose ();
				btnInfo = null;
			}
			if (btnToggleLamp != null) {
				btnToggleLamp.Dispose ();
				btnToggleLamp = null;
			}
			if (imgViewLamp != null) {
				imgViewLamp.Dispose ();
				imgViewLamp = null;
			}
			if (lblLevel != null) {
				lblLevel.Dispose ();
				lblLevel = null;
			}
			if (sliderBrightness != null) {
				sliderBrightness.Dispose ();
				sliderBrightness = null;
			}
		}
	}
}
