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

using UIKit;
using CoreGraphics;

namespace MultiModeLamp
{
	/// <summary>
	/// Simulator torch. Uses a small UIView which is placed at the top left corner of the screen.
	/// This is just there to allow development with the Simulator.
	/// </summary>
	public class SimulatorTorch : ITorch
	{
		UIView view;

		public SimulatorTorch ()
		{
			this.view = new UIView(new CGRect(0, 0, 1, 1));
			var parentView = UIApplication.SharedApplication.KeyWindow.RootViewController.View;
			parentView.Add(this.view);
			parentView.BringSubviewToFront(this.view);
		}

		public bool HasTorchInstalled
		{
			get
			{
				return true;
			}
		}

		public bool SetBrightness (float level)
		{
			if(!this.IsEnabled)
			{
				return true;
			}

			UIApplication.SharedApplication.InvokeOnMainThread(() => this.view.Alpha = level);
			return true;
		}

		public bool SetTorchEnabled (bool enabled)
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() => this.view.BackgroundColor = enabled ? UIColor.White : UIColor.Black);
			this.IsEnabled = enabled;
			return true;
		}

		public bool IsEnabled
		{
			get;
			private set;
		}
	}
}

