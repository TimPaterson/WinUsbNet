using System;
using System.Runtime.InteropServices;

namespace WinUsbNet
{
	///<summary >
	/// API declarations relating to device management (SetupDixxx and 
	/// RegisterDeviceNotification functions).   
	/// </summary>
	sealed internal partial class DeviceManagement
	{
		// from dbt.h

		internal const int DBT_DEVICEARRIVAL = 0X8000;
		internal const int DBT_DEVICEREMOVECOMPLETE = 0X8004;
		internal const int DBT_DEVTYP_DEVICEINTERFACE = 5;
		internal const int DBT_DEVTYP_HANDLE = 6;
		internal const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
		internal const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
		internal const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
		internal const int WM_DEVICECHANGE = 0X219;

		// from setupapi.h

		internal const int DIGCF_PRESENT = 2;
		internal const int DIGCF_DEVICEINTERFACE = 0X10;

		// Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.

		// Use this one in the call to RegisterDeviceNotification() and
		// in checking dbch_devicetype in a DEV_BROADCAST_HDR structure:

		// These are offsets within DEV_BROADCAST_DEVICEINTERFACE
		internal const int DBCC_DEVICETYPE_OFFSET =	sizeof(uint);
		internal const int DBCC_NAME_OFFSET = 3 * sizeof(uint) + 16 /* sizeof(Guid) */;

		[StructLayout(LayoutKind.Sequential)]
		internal struct DEV_BROADCAST_DEVICEINTERFACE
		{
			internal int dbcc_size;
			internal int dbcc_devicetype;
			internal int dbcc_reserved;
			internal Guid dbcc_classguid;
			internal Int16 dbcc_name;
		}

		//[StructLayout(LayoutKind.Sequential)]
		//internal class DEV_BROADCAST_HDR
		//{
		//    internal int dbch_size;
		//    internal int dbch_devicetype;
		//    internal int dbch_reserved;
		//}
		
		internal struct SP_DEVICE_INTERFACE_DATA
		{
			internal int cbSize;
			internal Guid InterfaceClassGuid;
			internal int Flags;
			internal IntPtr Reserved;
		}

		// This is an offset within SP_DEVICE_INTERFACE_DETAIL_DATA
		internal const int DEVICE_PATH_OFFSET = sizeof(uint);

		//internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
		//{
		//    internal int cbSize;
		//    internal String DevicePath;
		//}

		//internal struct SP_DEVINFO_DATA
		//{
		//    internal int cbSize;
		//    internal Guid ClassGuid;
		//    internal int DevInst;
		//    internal int Reserved;
		//}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr RegisterDeviceNotification(
			IntPtr hRecipient, 
			ref DEV_BROADCAST_DEVICEINTERFACE NotificationFilter, 
			int Flags);

		[DllImport("setupapi.dll", SetLastError = true)]
		internal static extern int SetupDiCreateDeviceInfoList(
			ref Guid ClassGuid, 
			int hwndParent);

		[DllImport("setupapi.dll", SetLastError = true)]
		internal static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

		[DllImport("setupapi.dll", SetLastError = true)]
		internal static extern bool SetupDiEnumDeviceInterfaces(
			IntPtr DeviceInfoSet, 
			IntPtr DeviceInfoData, 
			ref Guid InterfaceClassGuid, 
			uint MemberIndex, 
			out SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr SetupDiGetClassDevs(
			ref Guid ClassGuid, 
			IntPtr Enumerator, 
			IntPtr hwndParent, 
			uint Flags);

		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern bool SetupDiGetDeviceInterfaceDetail(
			IntPtr DeviceInfoSet, 
			ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, 
			IntPtr DeviceInterfaceDetailData, 
			uint DeviceInterfaceDetailDataSize, 
			out uint RequiredSize, 
			IntPtr DeviceInfoData);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Boolean UnregisterDeviceNotification(IntPtr Handle);
	}
}
