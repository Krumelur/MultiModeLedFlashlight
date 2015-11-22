using System;

namespace MultiModeLamp
{
	/// <summary>
	/// Interface to abstract flash usage. 
	/// The Simulator does not support accessing the flash and crashes.
	/// </summary>
	public interface ITorch
	{
		/// <summary>
		/// Sets the brightness.
		/// </summary>
		/// <returns><c>true</c>, if brightness was set, <c>false</c> otherwise.</returns>
		/// <param name="level">Brightness in percent</param>
		bool SetBrightness(float level);

		/// <summary>
		/// Sets the torch enabled.
		/// </summary>
		/// <returns><c>true</c>, if torch enabled was set, <c>false</c> otherwise.</returns>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		bool SetTorchEnabled(bool enabled);

		/// <summary>
		/// Gets a value indicating whether the torch is currently on.
		/// </summary>
		bool IsEnabled { get; }

		/// <summary>
		/// Gets a value indicating whether a torch is available on the device.
		/// </summary>
		bool HasTorchInstalled { get; }
	}
}

