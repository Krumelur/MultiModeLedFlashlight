using System;
using AVFoundation;
using System.Linq;
using Foundation;
using Xamarin;

namespace MultiModeLamp
{
	public class FlashTorch : ITorch
	{
		readonly AVCaptureDevice torchDevice;

		public FlashTorch ()
		{
			this.torchDevice = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video)?.FirstOrDefault (d => d.HasTorch);		
		}

		public bool HasTorchInstalled
		{
			get
			{
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
			if(this.torchDevice.TorchMode == AVCaptureTorchMode.Off)
			{
				return true;
			}

			if(level < 0.1f)
			{
				level = 0.1f;
			}
			if(level > 1f)
			{
				level = 1f;
			}

			NSError error;

			// Set brightness.
			try
			{
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

				this.torchDevice.UnlockForConfiguration();

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

