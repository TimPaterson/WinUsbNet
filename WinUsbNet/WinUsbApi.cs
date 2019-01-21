using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace WinUsbNet
{
	class WinUsbApi
	{
		internal const uint DEVICE_SPEED = 1;

		internal const byte PipeMask = 0x7F;
		internal const byte ReadFlag = 0x80;
		internal const byte WriteFlag = 0;

		internal enum POLICY_TYPE
		{
			SHORT_PACKET_TERMINATE = 1,
			AUTO_CLEAR_STALL,
			PIPE_TRANSFER_TIMEOUT,
			IGNORE_SHORT_PACKETS,
			ALLOW_PARTIAL_READS,
			AUTO_FLUSH,
			RAW_IO,
		}

		internal enum USBD_PIPE_TYPE
		{
			UsbdPipeTypeControl,
			UsbdPipeTypeIsochronous,
			UsbdPipeTypeBulk,
			UsbdPipeTypeInterrupt,
		}

		internal enum USB_DEVICE_SPEED
		{
			UsbLowSpeed = 1,
			UsbFullSpeed,
			UsbHighSpeed,
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct USB_CONFIGURATION_DESCRIPTOR
		{
			internal byte bLength;
			internal byte bDescriptorType;
			internal ushort wTotalLength;
			internal byte bNumInterfaces;
			internal byte bConfigurationValue;
			internal byte iConfiguration;
			internal byte bmAttributes;
			internal byte MaxPower;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct USB_INTERFACE_DESCRIPTOR
		{
			internal byte bLength;
			internal byte bDescriptorType;
			internal byte bInterfaceNumber;
			internal byte bAlternateSetting;
			internal byte bNumEndpoints;
			internal byte bInterfaceClass;
			internal byte bInterfaceSubClass;
			internal byte bInterfaceProtocol;
			internal byte iInterface;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct WINUSB_PIPE_INFORMATION
		{
			internal USBD_PIPE_TYPE PipeType;
			internal byte PipeId;
			internal ushort MaximumPacketSize;
			internal byte Interval;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct WINUSB_SETUP_PACKET
		{
			internal byte RequestType;
			internal byte Request;
			internal ushort Value;
			internal ushort Index;
			internal ushort Length;
		}

		[DllImport("winusb.dll", SetLastError = true)]
		internal static extern bool WinUsb_ControlTransfer(
			IntPtr InterfaceHandle, 
			WINUSB_SETUP_PACKET SetupPacket, 
			byte[] Buffer, 
			uint BufferLength, 
			out uint LengthTransferred, 
			IntPtr Overlapped);

		[DllImport("winusb.dll", SetLastError = true)]
		internal static extern bool WinUsb_FlushPipe(
			IntPtr InterfaceHandle, 
			byte PipeID);

		[DllImport("winusb.dll", SetLastError = true)]
		internal static extern bool WinUsb_Free(IntPtr InterfaceHandle);

		[DllImport("winusb.dll", SetLastError = true)]
		internal static extern bool WinUsb_Initialize(
			SafeFileHandle DeviceHandle, 
			out IntPtr InterfaceHandle);

		//  Use this declaration to retrieve DEVICE_SPEED (the only currently defined InformationType).

		[DllImport("winusb.dll", SetLastError = true)]
		internal static extern bool WinUsb_QueryDeviceInformation(
			IntPtr InterfaceHandle, 
			uint InformationType, 
			ref uint BufferLength, 
			ref byte Buffer);

		[DllImport("winusb.dll", SetLastError = true)]
		internal static extern bool WinUsb_QueryInterfaceSettings(
			IntPtr InterfaceHandle, 
			byte AlternateInterfaceNumber, 
			out USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

		[DllImport("winusb.dll", SetLastError = true)]
		internal static extern bool WinUsb_QueryPipe(
			IntPtr InterfaceHandle, 
			byte AlternateInterfaceNumber, 
			byte PipeIndex, 
			out WINUSB_PIPE_INFORMATION PipeInformation);

		[DllImport("winusb.dll", SetLastError = true)]
		internal static extern bool WinUsb_ReadPipe(
			IntPtr InterfaceHandle, 
			byte PipeID, 
			IntPtr Buffer, 
			uint BufferLength, 
			out uint LengthTransferred, 
			IntPtr Overlapped);

		[DllImport("winusb.dll", SetLastError = true, EntryPoint = "WinUsb_ReadPipe")]
		internal static extern bool WinUsb_ReadPipeByte(
			IntPtr InterfaceHandle, 
			byte PipeID,
			out byte bData, 
			uint BufferLength, 
			out uint LengthTransferred, 
			IntPtr Overlapped);

		[DllImport("winusb.dll", SetLastError = true, EntryPoint = "WinUsb_SetPipePolicy")]
		internal static extern bool WinUsb_SetPipePolicyBool(
			IntPtr InterfaceHandle, 
			byte PipeID, 
			uint PolicyType, 
			uint ValueLength, 
			ref bool Value);

		[DllImport("winusb.dll", SetLastError = true, EntryPoint = "WinUsb_SetPipePolicy")]
		internal static extern bool WinUsb_SetPipePolicyTimeout(
			IntPtr InterfaceHandle, 
			byte PipeID, 
			uint PolicyType, 
			uint ValueLength, 
			ref uint Value);

		[DllImport("winusb.dll", SetLastError = true, EntryPoint = "WinUsb_GetPipePolicy")]
		internal static extern bool WinUsb_GetPipePolicyByte(
			IntPtr InterfaceHandle, 
			byte PipeID, 
			uint PolicyType, 
			ref uint ValueLength, 
			out byte Value);

		[DllImport("winusb.dll", SetLastError = true, EntryPoint = "WinUsb_GetPipePolicy")]
		internal static extern bool WinUsb_GetPipePolicyTimeout(
			IntPtr InterfaceHandle, 
			byte PipeID, 
			uint PolicyType, 
			ref uint ValueLength, 
			out uint Value);

		[DllImport("winusb.dll", SetLastError = true)]
		internal static extern bool WinUsb_WritePipe(
			IntPtr InterfaceHandle, 
			byte PipeID,
			IntPtr Buffer,
			uint BufferLength, 
			out uint LengthTransferred, 
			IntPtr Overlapped);

		[DllImport("winusb.dll", SetLastError = true, EntryPoint = "WinUsb_WritePipe")]
		internal static extern bool WinUsb_WritePipeByte(
			IntPtr InterfaceHandle, 
			byte PipeID, 
			ref byte bData, 
			uint BufferLength, 
			out uint LengthTransferred, 
			IntPtr Overlapped);
	}
}
