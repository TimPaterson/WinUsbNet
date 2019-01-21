using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace WinUsbNet
{
	sealed internal partial class DeviceManagement
	{
		static internal string[] GetDeviceNames(Guid guid)
		{
			List<string> lstNames;
			IntPtr hDevInfo;
			IntPtr pNameBuf;
			uint index;
			uint cbBuf;
			bool retval;
			string strName;
			SP_DEVICE_INTERFACE_DATA dataIface;

			dataIface.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));
			lstNames = new List<string>();

			// Get the device info list
			hDevInfo = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

			for (index = 0; ; index++)
			{
				// Get the next device
				retval = SetupDiEnumDeviceInterfaces(
					hDevInfo,
					IntPtr.Zero,
					ref guid,
					index,
					out dataIface);

				if (!retval)
					break;

				// See how long the name is
				retval = SetupDiGetDeviceInterfaceDetail(
					hDevInfo,
					ref dataIface,
					IntPtr.Zero,
					0,
					out cbBuf,
					IntPtr.Zero);

				// Allocate memory for the SP_DEVICE_INTERFACE_DETAIL_DATA structure using the returned buffer size.

				pNameBuf = Marshal.AllocHGlobal((int)cbBuf);

				// Store cbSize in the first bytes of the array. The number of bytes varies with 32- and 64-bit systems.

				Marshal.WriteInt32(pNameBuf, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
				
				// Call SetupDiGetDeviceInterfaceDetail again.
				// This time, pass a pointer to the name buffer
				// and the returned required buffer size.

				retval = SetupDiGetDeviceInterfaceDetail(
					hDevInfo,
					ref dataIface,
					pNameBuf,
					cbBuf,
					out cbBuf,
					IntPtr.Zero);


				// Get the String containing the devicePathName.
				strName = Marshal.PtrToStringAuto((IntPtr)(pNameBuf.ToInt64() + DeviceManagement.DEVICE_PATH_OFFSET));
				Marshal.FreeHGlobal(pNameBuf);

				lstNames.Add(strName);
			}

			SetupDiDestroyDeviceInfoList(hDevInfo);
			return lstNames.ToArray();
		}

		static internal IntPtr RegisterForDeviceNotifications(IntPtr formHandle, Guid classGuid)
		{
			DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface;

			// Set the parameters in the DEV_BROADCAST_DEVICEINTERFACE structure.
			devBroadcastDeviceInterface.dbcc_size = Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE));
			devBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
			devBroadcastDeviceInterface.dbcc_classguid = classGuid;

			// Initialize unused fields
			devBroadcastDeviceInterface.dbcc_reserved = 0;
			devBroadcastDeviceInterface.dbcc_name = 0;

			// ***
			//  API function

			//  summary
			//  Request to receive notification messages when a device in an interface class
			//  is attached or removed.

			//  parameters 
			//  Handle to the window that will receive device events.
			//  Pointer to a DEV_BROADCAST_DEVICEINTERFACE to specify the type of 
			//  device to send notifications for.
			//  DEVICE_NOTIFY_WINDOW_HANDLE indicates the handle is a window handle.

			//  Returns
			//  Device notification handle or NULL on failure.
			// ***

			return RegisterDeviceNotification(formHandle, ref devBroadcastDeviceInterface, DEVICE_NOTIFY_WINDOW_HANDLE);
		}
	}
}
