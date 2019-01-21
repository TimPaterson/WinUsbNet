using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace WinUsbNet
{
	///  <summary>
	///  API declarations relating to file I/O (and used by WinUsb).
	///  </summary>

	sealed internal class FileIO
	{
		internal const int FILE_ATTRIBUTE_NORMAL = 0X80;
		internal const int FILE_FLAG_OVERLAPPED = 0X40000000;
		internal const int FILE_SHARE_READ = 1;
		internal const int FILE_SHARE_WRITE = 2;
		internal const uint GENERIC_READ = 0X80000000;
		internal const uint GENERIC_WRITE = 0X40000000;
		internal const int INVALID_HANDLE_VALUE = -1;
		internal const int OPEN_EXISTING = 3;		

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern SafeFileHandle CreateFile(
			string lpFileName, 
			uint dwDesiredAccess, 
			uint dwShareMode, 
			IntPtr lpSecurityAttributes, 
			uint dwCreationDisposition, 
			uint dwFlagsAndAttributes,
			IntPtr hTemplateFile);
	}

}
