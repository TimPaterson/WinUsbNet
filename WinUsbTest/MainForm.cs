using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PatersonTech;
using WinUsbNet;
using WinUsbTest.Properties;

namespace WinUsbTest
{
	partial class MainForm : Form
	{
		#region Constants

		const string MsgErrorCaption = "WinUSB Test Error";
		const string MsgHexValue = "Invalid hex value.";
		const string MsgInvalidGuid = "Invalid GUID.";
		const string MsgInvalidTimeout = "Timeout value invalid.";
		const string MsgAttach = "Attached";
		const string MsgDetach = "Detached";
		const string MsgFound = "Device attached.";
		const string MsgNotFound = "Device not found.";
		const string MsgDetached = "Device detached.";
		const string MsgOpenError = "Unable to open device: ";

		static Color clrError = Color.Red;

		// Tool tips
		const string TipGuid = "Cut and paste the GUID from the driver INF file";
		const string TipFind = "Look for USB devices with this GUID";
		const string TipLength = "Maximum number of bytes to read";
		const string TipType = "bmRequestType field. Common values:\n" +
			"0 = device\n" + 
			"1 = interface\n" + 
			"2 = endpoint";
		const string TipRequest = "bRequest field. Common values:\n" +
			"0 = Get Status\n" + 
			"1 = Clear Feature\n" +
			"3 = Set Feature\n" +
			"6 = Get Descriptor\n" +
			"7 = Set Descriptor\n" +
			"8 = Get Configuration\n" +
			"9 = Set Configuration\n" +
			"A = Get Interface\n" +
			"B = Set Interface";
		const string TipValue = "wValue field. For descriptors:\n" +
			"100 = Device Descriptor\n" +
			"200 = Configuration Descriptor\n" +
			"30x = String Descriptor x";
		const string TipIndex = "wIndex field. For string descriptors, language ID.";
		const string TipReadUseShortPacket = 
			"When True, a short packet indicates all data has been sent, and \n" + 
			"the read operation is terminated.\n\n" + 
			"When False, reading continues after receiving a short packet until \n" +
			"the specified number of bytes are received or the read times out.";
		const string TipWriteUseShortPacket = 
			"When True, a write operation that is a multiple of the maximum \n" + 
			"packet size for the endpoint is terminated with a zero-length packet.\n\n" +
			"When False, a zero-length packet is never added to a write operation. \n" + 
			"The USB device must know in advance the size of the transfer, or the \n" +
			"length must never be a multiple of the maximum packet size.";
		const string TipEndpoint = "Endpoint for data transfer";
		const string TipDirection = "Direction of transfer";
		const string TipTransfer = "Execute data transfer";
		const string TipOutData = "Data to send, as hex bytes";
		const string TipClearBuf = "Clear read buffer";
		const string TipReadTimeout = "Time to wait for read data";

		#endregion


		#region Types

		enum MsgType
		{
			MSG_Info,
			MSG_In,
			MSG_Out,
			MSG_Err
		}

		class EndpointItem
		{
			public string Name;
			public int PipeId;

			public EndpointItem(string name, int pipeId)
			{
				Name = name;
				PipeId = pipeId;
			}

			public override string ToString()
			{
				return Name;
			}
		}

		#endregion


		#region Fields

		WinUsbManager m_WinUsb;
		UsbDevice m_Device;

		#endregion


		#region Constructor
		
		public MainForm()
		{
			InitializeComponent();
			radIn.Checked = true;
			DeviceDetached();
		}

		#endregion


		#region Private Methods

		void FindDevice()
		{
			try
			{
				Guid guid;

				guid = new Guid(txtGuid.Text);
				Settings.Default.Guid = guid;

				if (m_WinUsb != null)
					m_WinUsb.Dispose();
				m_WinUsb = new WinUsbManager(guid);
			}
			catch
			{
				LogMsg(MsgInvalidGuid, MsgType.MSG_Err);
				return;
			}

			if (m_WinUsb.UsbDevices.Count != 0)
				DeviceAttached();
			else
				LogMsg(MsgNotFound, MsgType.MSG_Info);

			m_WinUsb.DeviceChange += OnDeviceChange;
		}

		void DeviceAttached()
		{
			foreach (UsbDevice dev in m_WinUsb.UsbDevices)
			{
				if (DeviceAttached(dev))
					return;
			}
		}

		bool DeviceAttached(UsbDevice dev)
		{
			string str;

			try
			{
				dev.Open();
			}
			catch (Exception exc)
			{
				LogMsg(MsgOpenError + exc.Message, MsgType.MSG_Err);
				return false;
			}

			m_Device = dev;

			drpEndpoint.Items.Clear();
			drpEndpoint.Items.Add(new EndpointItem("Control", -1));

			foreach (PipeStream pipe in m_Device.PipeStreams)
			{
				if (pipe == null)
					continue;

				str = pipe.PipeId.ToString();
				if (pipe.CanRead && pipe.CanWrite)
					str += " (In, Out)";
				else if (pipe.CanRead)
					str += " (In)";
				else if (pipe.CanWrite)
					str += " (Out)";

				drpEndpoint.Items.Add(new EndpointItem(str, pipe.PipeId));
			}
			btnXfer.Enabled = true;
			drpEndpoint.Enabled = true;
			lblAttach.Text = MsgAttach;
			drpEndpoint.SelectedIndex = 0;
			LogMsg(MsgFound, MsgType.MSG_Info);
			return true;
		}

		void DeviceDetached()
		{
			drpEndpoint.Items.Clear();
			drpEndpoint.Enabled = false;
			radIn.Enabled = false;
			radOut.Enabled = false;
			btnXfer.Enabled = false;
			grpEndpoint.Enabled = false;
			grpSetup.Enabled = false;
			txtOutData.Enabled = false;
			txtLength.Enabled = false;
			lblAttach.Text = MsgDetach;
			m_Device = null;
		}

		void LogMsg(string strMsg, MsgType type)
		{
			Color	clrMsg;

			switch (type)
			{
			case MsgType.MSG_In:
				clrMsg = lblInData.ForeColor;
				break;

			case MsgType.MSG_Out:
				clrMsg = lblOutData.ForeColor;
				break;

			case MsgType.MSG_Err:
				clrMsg = clrError;
				break;

			default:
				clrMsg = Color.FromKnownColor(KnownColor.WindowText);
				break;
			}

			txtLog.SelectionColor = clrMsg;
			txtLog.AppendText(strMsg + '\n');
			txtLog.ScrollToCaret();
		}

		void LogData(byte[] arbData, int count, MsgType type)
		{
			string strMsg;
			byte b;

			if (arbData == null)
				return;

			if (count > arbData.Length)
				count = arbData.Length;

			if (count > 0)
			{
				strMsg = "";
				for (int i = 0; i < count; i++)
				{
					b = arbData[i];
					strMsg += b.ToString("X2") + " ";
				}
				LogMsg(strMsg, type);
			}
		}

		byte ReadHexByte(TextBox textbox)
		{
			string str;

			str = textbox.Text;

			if (!Regex.IsMatch(str, "^[0-9a-fA-F]{1,2}$"))
			{
				MessageBox.Show(MsgHexValue, MsgErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				textbox.SelectionStart = 0;
				textbox.SelectionLength = str.Length;
				throw new Exception();
			}
			return byte.Parse(str, NumberStyles.AllowHexSpecifier);
		}

		ushort ReadHexWord(TextBox textbox)
		{
			string str;

			str = textbox.Text;

			if (!Regex.IsMatch(str, "^[0-9a-fA-F]{1,4}$"))
			{
				MessageBox.Show(MsgHexValue, MsgErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				textbox.SelectionStart = 0;
				textbox.SelectionLength = str.Length;
				throw new Exception();
			}
			return ushort.Parse(str, NumberStyles.AllowHexSpecifier);
		}

		ushort ReadInt(TextBox textbox)
		{
			string str;

			str = textbox.Text;

			if (!Regex.IsMatch(str, "^[0-9]{1,6}$"))
			{
				MessageBox.Show(MsgHexValue, MsgErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				textbox.SelectionStart = 0;
				textbox.SelectionLength = str.Length;
				throw new Exception();
			}
			return ushort.Parse(str);
		}

		byte[] ReadOutData()
		{
			byte[] arbData;
			string str;
			Match match;
			Group group;
			int i;


			str = txtOutData.Text.Trim();
			if (str.Length == 0)
				return new byte[0];

			match = Regex.Match(str, "^(?:([0-9a-fA-F]{1,2}) *[ ,] *)*([0-9a-fA-F]{1,2})$");
			if (!match.Groups[0].Success)
			{
				MessageBox.Show(MsgHexValue, MsgErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtOutData.SelectionStart = 0;
				txtOutData.SelectionLength = txtOutData.Text.Length;
				throw new Exception();
			}

			group = match.Groups[1];
			i = 0;
			if (group.Success)
			{
				arbData = new byte[group.Captures.Count + 1];
				for (; i < group.Captures.Count; i++)
					arbData[i] = byte.Parse(group.Captures[i].Value, NumberStyles.AllowHexSpecifier);
			}
			else
				arbData = new byte[1];

			arbData[i] = byte.Parse(match.Groups[2].Value, NumberStyles.AllowHexSpecifier);
			return arbData;
		}

		#endregion


		#region Event Handlers

		void OnDeviceChange(object sender, DeviceChangeEventArgs e)
		{
			if (e.IsAttach)
			{
				if (m_Device == null)
					DeviceAttached(e.UsbDevice);
			}
			else if (e.UsbDevice == m_Device)
			{
				DeviceDetached();
				LogMsg(MsgDetached, MsgType.MSG_Info);
				if (m_WinUsb.UsbDevices.Count != 0)
					DeviceAttached();	// Use different device
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			// Read in saved per-user settings
			if (!Settings.Default.SettingsUpgraded)
			{
				Settings.Default.Upgrade();
				Settings.Default.SettingsUpgraded = true;
				Settings.Default.Save();
			}

			// Don't use some settings if they are default values
			if (Settings.Default.MainForm != null)
			{
				Settings.Default.MainForm.RestoreForm(this);
				txtGuid.Text = Settings.Default.Guid.ToString();
			}

			// Tool tips
			toolTip.InitialDelay = 200;
			toolTip.AutoPopDelay = 20000;

			toolTip.SetToolTip(txtGuid, TipGuid);
			toolTip.SetToolTip(btnFind, TipFind);
			toolTip.SetToolTip(txtLength, TipLength);
			toolTip.SetToolTip(lblLength, TipLength);
			toolTip.SetToolTip(txtType, TipType);
			toolTip.SetToolTip(lblType, TipType);
			toolTip.SetToolTip(txtRequest, TipRequest);
			toolTip.SetToolTip(lblRequest, TipRequest);
			toolTip.SetToolTip(txtValue, TipValue);
			toolTip.SetToolTip(lblValue, TipValue);
			toolTip.SetToolTip(txtIndex, TipIndex);
			toolTip.SetToolTip(lblIndex, TipIndex);
			toolTip.SetToolTip(chkReadUseShortPacket, TipReadUseShortPacket);
			toolTip.SetToolTip(chkWriteUseShortPacket, TipWriteUseShortPacket);
			toolTip.SetToolTip(lblEndpoint, TipEndpoint);
			toolTip.SetToolTip(drpEndpoint, TipEndpoint);
			toolTip.SetToolTip(radIn, TipDirection);
			toolTip.SetToolTip(radOut, TipDirection);
			toolTip.SetToolTip(btnXfer, TipTransfer);
			toolTip.SetToolTip(txtOutData, TipOutData);
			toolTip.SetToolTip(lblOutDataList, TipOutData);
			toolTip.SetToolTip(btnClear, TipClearBuf);
			toolTip.SetToolTip(txtReadTimeout, TipReadTimeout);
			toolTip.SetToolTip(lblReadTimeout, TipReadTimeout);
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.Default.MainForm = new FormSettings(this);

			Settings.Default.Save();
			if (m_WinUsb != null)
				m_WinUsb.Dispose();
		}

		private void drpEndpoint_SelectedIndexChanged(object sender, EventArgs e)
		{
			EndpointItem item;
			PipeStream	pipe;

			item = (EndpointItem)drpEndpoint.SelectedItem;
			if (item.PipeId == -1)
			{
				// Control endpoint
				grpSetup.Enabled = true;
				grpEndpoint.Enabled = false;
				radIn.Enabled = true;
				radOut.Enabled = true;
				chkReadUseShortPacket.Checked = true;
				chkWriteUseShortPacket.Checked = true;
				txtReadTimeout.Text = string.Empty;
				btnClear.Enabled = false;
			}
			else
			{
				grpSetup.Enabled = false;
				grpEndpoint.Enabled = true;
				pipe = m_Device.PipeStreams[item.PipeId];

				if (pipe.CanRead)
				{
					radIn.Enabled = true;
					chkReadUseShortPacket.Enabled = true;
					txtReadTimeout.Enabled = true;
					chkReadUseShortPacket.Checked = pipe.ReadUseShortPacket;
					txtReadTimeout.Text = pipe.ReadTimeout.ToString();
					btnClear.Enabled = true;
				}
				else
				{
					radOut.Checked = true;
					radIn.Enabled = false;
					chkReadUseShortPacket.Checked = false;
					chkReadUseShortPacket.Enabled = false;
					txtReadTimeout.Text = string.Empty;
					txtReadTimeout.Enabled = false;
					btnClear.Enabled = false;
				}

				if (pipe.CanWrite)
				{
					radOut.Enabled = true;
					chkWriteUseShortPacket.Enabled = true;
					chkWriteUseShortPacket.Checked = pipe.WriteUseShortPacket;
				}
				else
				{
					radIn.Checked = true;
					radOut.Enabled = false;
					chkWriteUseShortPacket.Checked = false;
					chkWriteUseShortPacket.Enabled = false;
				}
			}
			radIn_CheckedChanged(sender, e);
		}

		private void radIn_CheckedChanged(object sender, EventArgs e)
		{
			txtLength.Enabled = radIn.Checked;
			txtOutData.Enabled = !radIn.Checked;
		}

		private void btnXfer_Click(object sender, EventArgs e)
		{
			byte bmRequestType;
			byte bRequest;
			byte idPipe;
			ushort wValue;
			ushort wIndex;
			ushort wLength;
			byte[] arbData;
			PipeStream pipe;
			EndpointItem item;
			int cbRead;

			try
			{
				item = (EndpointItem)drpEndpoint.SelectedItem;
				if (item.PipeId == -1)
				{
					// Control transfer
					bmRequestType = ReadHexByte(txtType);
					bRequest = ReadHexByte(txtRequest);
					wValue = ReadHexWord(txtValue);
					wIndex = ReadHexWord(txtIndex);
					if (radIn.Checked)
					{
						wLength = ReadInt(txtLength);
						arbData = m_Device.ControlRead(bmRequestType, bRequest, wValue, wIndex, wLength);
						// Check for the case of getting a string descriptor
						bmRequestType |= 0x80;
						wValue >>= 8;
						if (bmRequestType == 0x80 && bRequest == 6 && wValue == 3 && arbData.Length > 2)
						{
							string str = Encoding.Unicode.GetString(arbData, 2, arbData.Length - 2);
							LogMsg(str, MsgType.MSG_In);
						}
						else
							LogData(arbData, arbData.Length, MsgType.MSG_In);
					}
					else
					{
						arbData = ReadOutData();
						wLength = (ushort)arbData.Length;
						m_Device.ControlWrite(bmRequestType, bRequest, wValue, wIndex, arbData);
						LogData(arbData, arbData.Length, MsgType.MSG_Out);
					}
				}
				else
				{
					// bulk data transfer
					idPipe = (byte)item.PipeId;
					pipe = m_Device.PipeStreams[idPipe];

					if (radIn.Checked)
					{
						wLength = ReadInt(txtLength);
						arbData = new byte[wLength];
						if (wLength == 1)
						{
							// Test ReadByte()
							cbRead = pipe.ReadByte();
							if (cbRead == -1)
								cbRead = 0;
							else
							{
								arbData[0] = (byte)cbRead;
								cbRead = 1;
							}
						}
						else
							cbRead = pipe.Read(arbData, 0, arbData.Length);

						LogData(arbData, cbRead, MsgType.MSG_In);
					}
					else
					{
						arbData = ReadOutData();
						if (arbData.Length == 1)
							pipe.WriteByte(arbData[0]);
						else
							pipe.Write(arbData, 0, arbData.Length);

						LogData(arbData, arbData.Length, MsgType.MSG_Out);
					}
				}
			}
			catch (Exception exc)
			{
				LogMsg(exc.Message, MsgType.MSG_Info);
				return;
			}
		}

		private void btnFind_Click(object sender, EventArgs e)
		{
			FindDevice();
		}

		private void txtReadTimeout_Leave(object sender, EventArgs e)
		{
			int timeout;
			EndpointItem item;
			PipeStream pipe;

			timeout = ReadInt(txtReadTimeout);
			item = (EndpointItem)drpEndpoint.SelectedItem;
			pipe = m_Device.PipeStreams[item.PipeId];
			pipe.ReadTimeout = timeout;
			// Read it back
			txtReadTimeout.Text = pipe.ReadTimeout.ToString();
		}

		private void chkReadUseShortPacket_Click(object sender, EventArgs e)
		{
 			EndpointItem item;
			PipeStream pipe;

			item = (EndpointItem)drpEndpoint.SelectedItem;
			pipe = m_Device.PipeStreams[item.PipeId];
			pipe.ReadUseShortPacket = chkReadUseShortPacket.Checked;
			// Read it back
			chkReadUseShortPacket.Checked = pipe.ReadUseShortPacket;
		}

		private void chkWriteUseShortPacket_Click(object sender, EventArgs e)
		{
 			EndpointItem item;
			PipeStream pipe;

			item = (EndpointItem)drpEndpoint.SelectedItem;
			pipe = m_Device.PipeStreams[item.PipeId];
			pipe.WriteUseShortPacket = chkWriteUseShortPacket.Checked;
			// Read it back
			chkWriteUseShortPacket.Checked = pipe.WriteUseShortPacket;
		}

		private void txtLog_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			txtLog.Clear();
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			EndpointItem item;

			item = (EndpointItem)drpEndpoint.SelectedItem;
			if (item.PipeId == -1)
				return;

			m_Device.PipeStreams[item.PipeId].ClearReadBuffer();
		}

		#endregion
	}
}
