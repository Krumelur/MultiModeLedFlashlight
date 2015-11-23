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
using AVFoundation;
using System.Linq;
using Foundation;
using Xamarin;

namespace MultiModeLamp
{
	/// <summary>
	/// Gets access to the device's flash/torch.
	/// </summary>
	public class FlashTorch : ITorch
	{
		// Reference to the capture device with flash support.
		readonly AVCaptureDevice torchDevice;

		public FlashTorch ()
		{
			// Check if there are any devices with torch support.
			this.torchDevice = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video)?.FirstOrDefault (d => d.HasTorch);		
		}

		public bool HasTorchInstalled
		{
			get {
				return this.torchDevice != null;
			}
		}

		public bool SetBrightness (float level)
		{
			if (this.torchDevice == null)
			{
				return false;
			}

			// Setting brightness turns the lamp on - I don't want this behavior here. I want the brightness to act only if the lamp is on and use the current
			// brightness level as the initial value.
			if (this.torchDevice.TorchMode == AVCaptureTorchMode.Off)
			{
				return true;
			}

			if (level < 0.1f)
			{
				level = 0.1f;
			}
			if (level > 1f)
			{
				level = 1f;
			}

			NSError error;

			// Set brightness.
			try
			{
				// Must always lock the device exclusively.
				bool success = this.torchDevice.LockForConfiguration (out error);
				if (!success || error != null)
				{
					Insights.Report (new InvalidOperationException (error?.Description), Insights.Severity.Error);
				}
				success = this.torchDevice.SetTorchModeLevel (level, out error);

				if (!success || error != null)
				{
					Insights.Report (new InvalidOperationException (error?.Description), Insights.Severity.Error);
				}

				this.torchDevice.UnlockForConfiguration ();

				return success;
			}
			catch (Exception ex)
			{
				Insights.Report (ex, Insights.Severity.Error);
				return false;
			}
		}

		public bool SetTorchEnabled (bool enabled)
		{
			if (this.torchDevice == null)
			{
				return false;
			}
			NSError error;
			bool success = torchDevice.LockForConfiguration (out error);
			if (success && error == null)
			{
				torchDevice.TorchMode = enabled ? AVCaptureTorchMode.On : AVCaptureTorchMode.Off;
				torchDevice.UnlockForConfiguration ();
				return true;
			}

			return false;
		}

		public bool IsEnabled
		{
			get {
				return this.torchDevice.TorchMode == AVCaptureTorchMode.On;
			}
		}
	}
}

