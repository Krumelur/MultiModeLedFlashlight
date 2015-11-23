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
		bool SetBrightness (float level);

		/// <summary>
		/// Sets the torch enabled.
		/// </summary>
		/// <returns><c>true</c>, if torch enabled was set, <c>false</c> otherwise.</returns>
		/// <param name="enabled">If set to <c>true</c> enabled.</param>
		bool SetTorchEnabled (bool enabled);

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

