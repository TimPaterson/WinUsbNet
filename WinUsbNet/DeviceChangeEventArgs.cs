﻿using System;

namespace WinUsbNet
{
	/// <summary>
	/// Provides data for the <see cref="WinUsbManager.DeviceChange"/> event.
	/// </summary>
	/// <remarks>
	/// Use the <see cref="IsAttach"/> member to determine if a USB device has
	/// just been attached or detached.
	/// </remarks>
	public class DeviceChangeEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceChangeEventArgs"/> class.
		/// </summary>
		/// <param name="deviceName">A string that identifies the USB device.</param>
		/// <param name="isAttach"><b>true</b> if attach, <b>false</b> if detach.</param>
		public DeviceChangeEventArgs(string deviceName, bool isAttach)
		{
			DeviceName = deviceName;
			IsAttach = isAttach;
			UsbDevice = null;
		}

		/// <summary>
		/// Gets the string generated by Windows that uniquely identifies the USB device.
		/// </summary>
		/// <value>
		/// Type: <see cref="System.String"/>
		/// <para>A string that uniquely identifies the USB device while it is attached.</para>
		/// </value>
		/// <remarks>
		/// The name string is not meaningful to users.
		/// </remarks>
		public string DeviceName { get; internal set; }

		/// <summary>
		/// Get a value that indicates if the <see cref="WinUsbManager.DeviceChange"/> event 
		/// is reporting device attach or detach.
		/// </summary>
		/// <value>
		/// Type: <see cref="System.Boolean"/>
		/// <para><b>true</b> if attach, <b>false</b> if detach.</para>
		/// </value>
		public bool IsAttach { get; internal set; }

		/// <summary>
		/// Gets the <see cref="WinUsbNet.UsbDevice"/> affected by the event.
		/// </summary>
		/// <value>
		/// Type: <see cref="WinUsbNet.UsbDevice"/>
		/// <para>The <see cref="WinUsbNet.UsbDevice"/> affected by the 
		/// <see cref="WinUsbManager.DeviceChange"/> event.</para>
		/// </value>
		/// <remarks>
		/// If a USB device has been attached (<see cref="IsAttach"/> is <b>true</b>),
		/// this field contains a newly created <see cref="WinUsbNet.UsbDevice"/>
		/// which has already been added to the <see cref="WinUsbManager.UsbDevices"/>
		/// collection of <see cref="WinUsbManager"/>.
		/// <para>
		/// If a USB device has been detached (<see cref="IsAttach"/> is <b>false</b>),
		/// this field contains the <see cref="WinUsbNet.UsbDevice"/> that has been
		/// removed from the <see cref="WinUsbManager.UsbDevices"/> collection of 
		/// <see cref="WinUsbManager"/>. The <see cref="WinUsbNet.UsbDevice"/> is closed and its 
		/// <see cref="WinUsbNet.UsbDevice.IsAttached"/> property is set to <b>false</b>. It
		/// cannot be opened again.
		/// </para>
		/// </remarks>
		public UsbDevice UsbDevice { get; internal set; }
	}
}
