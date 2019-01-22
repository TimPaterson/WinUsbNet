using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using WinUsbNet;

namespace WinUsbExample
{
	public partial class Form1 : Form
	{
		const string StrGuidDefault = "5eca211f-8f8f-4848-9c44-ca291c8dbb6a";

		// private instance of our WinUsbManager
		WinUsbManager m_WinUsb;

		// Form constructor
		public Form1()
		{
			InitializeComponent();
		}

		// Create a new WinUsbManager, disposing of any previous instance.
		// Called whenever the GUID changes.
		void CreateWinUsbManager(Guid guid)
		{
			if (m_WinUsb != null)
				m_WinUsb.Dispose();
			m_WinUsb = new WinUsbManager(guid);

			// Subscribe to the DeviceChange event
			m_WinUsb.DeviceChange += new EventHandler<DeviceChangeEventArgs>(DeviceChange);

			// If any devices are already attached, put them in the list
			foreach (UsbDevice dev in m_WinUsb.UsbDevices)
				GetDeviceName(dev);
		}

		// Helper function to get a string descriptor from the USB device.
		string GetStringDescriptor(UsbDevice dev, int iString)
		{
			byte[] arbBuf;

			if (iString != 0)
			{
				// Get the string descriptor:
				// bmRequestType = 0 - standard device request
				// bRequest = 6 - Get_Descriptor
				// wValue: high byte = 3 - string; low byte = descriptor index
				// wIndex = 0x409 - language ID, US English
				// wLength = 255 - max length
				arbBuf = dev.ControlRead(0, 6, (ushort)(iString | 0x300), 0x409, 255);

				// arbBuf[0] = total length
				// arbBuf[1] = 3 (string)
				// arbBuf[2] = start of string (Unicode)
				if (arbBuf.Length >= 4)
					return Encoding.Unicode.GetString(arbBuf, 2, arbBuf[0] - 2);
			}
			return string.Empty;
		}

		// Build up device name from manufacturer, product name, and
		// serial number strings. The index of each of these strings
		// is in a fixed location in the device descriptor. Then add
		// the device to the list box.
		void GetDeviceName(UsbDevice dev)
		{
			byte[] arbBuf;
			string strTag;

			try
			{
				dev.Open();

				// Get device descriptor
				// bmRequestType = 0 - standard device request
				// bRequest = 6 - Get_Descriptor
				// wValue: high byte = 1 - device; low byte = 0 - descriptor index
				// wIndex = 0 - not used
				// wLength = 18 - length of descriptor
				arbBuf = dev.ControlRead(0, 6, 0x100, 0, 18);

				// Entries 14, 15, and 16 have the index of the manufacturer,
				// product name, and serial number string descriptors, respectively.
				strTag = GetStringDescriptor(dev, arbBuf[14]);
				strTag += '/';
				strTag += GetStringDescriptor(dev, arbBuf[15]);
				strTag += '/';
				strTag += GetStringDescriptor(dev, arbBuf[16]);

				// Assign the combined ID string to the Tag.
				dev.Tag = strTag;
			}
			catch (Exception exc)
			{
				dev.Tag = "Can't open: " + exc.Message;
			}

			dev.Close();	// done for now

			// Use the Tag as the value to be returned by ToString().
			// This makes it easy to add the UsbDevice to a list box.
			dev.GetMyString = delegate(UsbDevice device) { return (string)device.Tag; };

			// The list box will display ToString(), which is now the Tag.
			lstDevices.Items.Add(dev);
		}

		//*****************************************************************
		// Events

		// Matching USB device has been attached or detached.
		void DeviceChange(object sender, DeviceChangeEventArgs e)
		{
			if (e.IsAttach)
				GetDeviceName(e.UsbDevice);	// Add name to list box
			else
				lstDevices.Items.Remove(e.UsbDevice); // Remove name from list box
		}

		// Form is displayed, set the GUID text box to the default value.
		// This will cause the TextChanged event to fire.
		private void Form1_Shown(object sender, EventArgs e)
		{
			txtGuid.Text = StrGuidDefault;
		}

		// GUID has changed. If valid, use it to create a new
		// WinUsbManager.
		private void txtGuid_TextChanged(object sender, EventArgs e)
		{
			Guid guid;

			lstDevices.Items.Clear();	// Empty the list box

			try
			{
				guid = new Guid(txtGuid.Text);
				CreateWinUsbManager(guid);
			}
			catch (Exception exc)
			{
				lstDevices.Items.Add("Invalid GUID: " + exc.Message);
			}
		}

		//*****************************************************************
		// I/O

		// Just for demo purposes, use the DoubleClick event on the list box
		// to trigger some I/O on the selected device.
		private void lstDevices_DoubleClick(object sender, EventArgs e)
		{
			UsbDevice dev = null;
			BinaryReader reader;
			BinaryWriter writer;

			try
			{
				// Open the USB device selected in the list box
				dev = (UsbDevice)lstDevices.SelectedItem;
				dev.Open();

				// Search for IN and OUT endpoints. Typically this is not 
				// necessary because you would know the characteristics of
				// your USB device (i.e., what endpoint to use).
				reader = null;
				writer = null;
				foreach (PipeStream pipe in dev.PipeStreams)
				{
					if (pipe == null)
						continue;

					// When we find a PipeStream that can read, wrap it in a
					// BinaryReader. Note that it is still OK to bypass it
					// and perform reads directly with the PipeStream.
					if (pipe.CanRead && reader == null)
						reader = new BinaryReader(pipe);

					// To use BinaryWriter, we need to wrap the PipeStream
					// in a BufferedStream. Otherwise each Write() call would
					// send its own little USB packet. If WriteUseShortPacket
					// is false, then the buffer size should just equal the
					// packet size. Otherwise, the buffer must be big enough
					// for the largest block we'll send through the 
					// BinaryWriter. Note that you can still bypass the 
					// BinaryWriter and go directly to the PipeStream; this
					// would be useful for large blocks.
					if (pipe.CanWrite && writer == null)
						writer = new BinaryWriter(
							new BufferedStream(pipe, pipe.WriteMaxPacketSize));
				}

				// Create some data
				byte b = 0;
				ushort us = 0;
				byte bCnt = 7;
				int i;

				// Write command data. By using BufferedStream, nothing is
				// sent until the Flush() call.
				writer.Write(b);
				writer.Write(us);
				writer.Write(bCnt);
				writer.Flush();

				// Read some data back
				us = reader.ReadUInt16();
				i = reader.ReadInt32();

				// Demonstrate bypassing the BinaryReader. Most likely you
				// would do this for larger blocks that you want as an
				// array of bytes.
				//b = reader.ReadByte();
				b = (byte)reader.BaseStream.ReadByte();

				// Just show data in debugger output window.
				Debug.WriteLine("Data: " + us + ", " + i + ", " + b);
			}
			catch (Exception exc)
			{
				MessageBox.Show("Transfer error: " + exc.Message, 
					"WinUSB Example", 
					MessageBoxButtons.OK, 
					MessageBoxIcon.Error);
			}
			finally
			{
				if (dev != null)
					dev.Close();
			}
		}
	}
}