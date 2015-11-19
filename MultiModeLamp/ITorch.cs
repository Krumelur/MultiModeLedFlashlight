using System;

namespace MultiModeLamp
{
	public interface ITorch
	{
		bool SetBrightness(float level);

		bool SetTorchEnabled(bool enabled);

		bool IsEnabled { get; }

		bool HasTorchInstalled { get; }
	}
}

