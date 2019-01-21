using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinUsbNet
{
	class DeviceNotifyControl : Control
	{
		internal event EventHandler<DeviceChangeEventArgs> DeviceChangeEvent;

		///  <summary>
		///  Overrides WndProc to enable checking for and handling
		///  WM_DEVICECHANGE messages.
		///  </summary>
		///  
		///  <param name="m"> A Windows message.
		///  </param> 
		///  
		protected override void WndProc(ref Message m)
		{
			int wParam;
			int dbcc_devicetype;
			string strDeviceName;

			if (m.Msg == DeviceManagement.WM_DEVICECHANGE)
			{
				wParam = m.WParam.ToInt32();
				if (wParam == DeviceManagement.DBT_DEVICEARRIVAL || 
					wParam == DeviceManagement.DBT_DEVICEREMOVECOMPLETE)
				{
					// lParam is pointer to a structure derived from DEV_BROADCAST_HDR
					dbcc_devicetype = Marshal.ReadInt32(m.LParam, DeviceManagement.DBCC_DEVICETYPE_OFFSET);
					if (dbcc_devicetype == DeviceManagement.DBT_DEVTYP_DEVICEINTERFACE)
					{
						strDeviceName = Marshal.PtrToStringUni((IntPtr)(m.LParam.ToInt64() + DeviceManagement.DBCC_NAME_OFFSET));
						if (DeviceChangeEvent != null)
							DeviceChangeEvent(this, new DeviceChangeEventArgs(strDeviceName, wParam == DeviceManagement.DBT_DEVICEARRIVAL));
					}
				}
			}

			// Let the base control process the message.
			base.WndProc(ref m);
		}
	}
}
