using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace WinUsbNet
{
	/// <summary>
	/// The collection of <see cref="PipeStream"/> objects corresponding to the
	/// USB communication pipes of a USB device.
	/// </summary>
	/// <remarks>
	/// When its parent <see cref="UsbDevice"/> is open, the <b>PipeStreamCollection</b>
	/// contains 16 entries, accessible through the <b>Item</b> property with 
	/// indices in the range 0 - 15. The index for an entry is the same as the 
	/// USB endpoint number for the corresponding USB pipe. If the USB device has
	/// no endpoint for a given index, then that entry in the collection is <b>null</b>.
	/// Otherwise, the entry contains a <see cref="PipeStream"/>.
	/// </remarks>
	public sealed class PipeStreamCollection : ReadOnlyCollection<PipeStream>
	{
		internal PipeStreamCollection(PipeStream[] arPipeStream)
			: base(arPipeStream)
		{
		}
	}

	/// <summary>
	/// Represents a matching USB device that has been attached to the host computer.
	/// </summary>
	/// <remarks>
	/// <b>UsbDevice</b> represents a USB device that has been attached and whose 
	/// WinUSB GUID matches the GUID of the parent <see cref="WinUsbManager"/>.
	/// You do not create a <b>UsbDevice</b> directly. A <b>UsbDevice</b> is
	/// automatically placed in the <see cref="WinUsbManager.UsbDevices"/>
	/// collection for each matching USB device that is attached.
	/// <para>
	/// A <b>UsbDevice</b> is valid only as long as the corresponding USB device
	/// remains attached to the host. Once it is been detached, the <b>UsbDevice</b>
	/// is closed if it was open, the <see cref="IsAttached"/> property is set
	/// to <b>false</b>, and calls to the <see cref="Open">Open</see> method will
	/// throw an exception. Reattaching the USB device will <b>not</b> make the 
	/// existing <b>UsbDevice</b> valid again. Instead a new <b>UsbDevice</b> 
	/// is created and added to the <see cref="WinUsbManager.UsbDevices"/> collection of the 
	/// parent <see cref="WinUsbManager"/>.</para>
	/// <para>
	/// To communicate with the USB device, you must first call the 
	/// <see cref="Open">Open</see> method. This will populate the 
	/// <see cref="PipeStreams"/> collection with <see cref="PipeStream"/> objects
	/// corresponding to the endpoints of the USB device. When you are 
	/// finished communicating with the USB device, call the 
	/// <see cref="Close">Close</see> method, which will set the
	/// <see cref="PipeStreams"/> property to <b>null</b>.</para>
	/// <para>
	/// You may open, transfer data, and close the <b>UsbDevice</b>
	/// any number of times. This can allow you to share the USB device 
	/// with multiple applications.</para>
	/// <para>
	/// When the <b>UsbDevice</b> is open, you can send and receive USB control
	/// transfers using the <see cref="ControlWrite">ControlWrite</see> and 
	/// <see cref="ControlRead">ControlRead</see> methods. To transfer
	/// data with other endpoints, use the endpoint number as an index into
	/// the <see cref="PipeStreams"/> collection, then use the returned
	/// <see cref="PipeStream"/> to transfer data.</para>
	/// <para>
	/// The index of a <b>UsbDevice</b> in the <see cref="WinUsbManager.UsbDevices"/>
	/// collection of the parent <see cref="WinUsbManager"/> can change
	/// as USB devices are attached and detached. <b>UsbDevice</b> includes
	/// a <see cref="Tag"/> property that can be assigned any generic object 
	/// to make it easier to track each <b>UsbDevice</b>.</para>
	/// </remarks>
	public sealed class UsbDevice
	{
		#region Constants

		static IntPtr INVALID_USB_HANDLE = (IntPtr)(-1);

		const int DefaultReadTimeoutValue = 100;

		const string ErrDeviceNotOpen = "Device not open.";
		const string ErrDeviceNotAttached = "Device not attached.";

		#endregion


		#region Types
		
		/// <summary>
		/// Represents a callback function used to get the string returned 
		/// by the <see cref="ToString">ToString</see> method on
		/// <see cref="UsbDevice"/>.
		/// </summary>
		/// <param name="dev">The <see cref="UsbDevice"/> object on which
		/// the <see cref="ToString">ToString</see> method is
		/// being called.</param>
		/// <returns>
		/// Type: <see cref="String"/>
		/// <para>The string to be returned by <see cref="ToString">ToString</see>.</para>
		/// </returns>
		/// <seealso cref="GetMyString"/>
		public delegate string GetMyStringCallback(UsbDevice dev);

		#endregion


		#region Constructor	& Destructor

		internal UsbDevice(string strDeviceName)
		{
			m_strDeviceName = strDeviceName;
			DefaultReadTimeout = DefaultReadTimeoutValue;
			m_hWinUsb = INVALID_USB_HANDLE;
			IsAttached = true;
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~UsbDevice()
		{
			Close();
		}

		#endregion


		#region Private Fields

		SafeFileHandle m_hDevice;
		IntPtr m_hWinUsb;
		string m_strDeviceName;
		PipeStream[] m_arPipeStreams;
		PipeStreamCollection m_collPipeStreams;
		
		#endregion


		#region Internal Properties

		internal IntPtr UsbHandle
		{
			get
			{
				if (!IsOpen)
					throw new IOException(ErrDeviceNotOpen);
				return m_hWinUsb;
			}
		}

		#endregion


		#region Public Properties

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
		public string DeviceName
		{
			get { return m_strDeviceName; }
		}

		/// <summary>
		/// Gets the collection of <see cref="PipeStream"/> objects corresponding to the
		/// USB communication pipes of a USB device.
		/// </summary>
		/// <value>
		/// Type: <see cref="PipeStreamCollection"/>
		/// <para>The collection of <see cref="PipeStream"/> objects representing
		/// the USB pipes of the device.</para>
		/// </value>
		/// <remarks>
		/// See <see cref="PipeStreamCollection"/> for more information.
		/// </remarks>
		/// <seealso cref="PipeStreamCollection"/>
		public PipeStreamCollection PipeStreams
		{
			get { return m_collPipeStreams; }
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="UsbDevice"/> is open or closed.
		/// </summary>
		/// <value>
		/// Type: <see cref="Boolean"/>
		/// <para><b>true</b> if the <see cref="UsbDevice"/> is open, otherwise <b>false</b>.</para>
		/// </value>
		public bool IsOpen
		{
			get { return m_hWinUsb != INVALID_USB_HANDLE; }
		}

		/// <summary>
		/// Gets a value indicating if this <see cref="UsbDevice"/> is valid.
		/// </summary>
		/// <value>
		/// Type: <see cref="Boolean"/>
		/// <para><b>true</b> if the <see cref="UsbDevice"/> is valid, 
		/// otherwise <b>false</b>.</para>
		/// </value>
		/// <remarks>
		/// The <b>IsAttached</b> property is initially <b>true</b>. It
		/// becomes <b>false</b> permanently if the USB device is detached.
		/// </remarks>
		public bool IsAttached { get; private set; }

		/// <summary>
		/// Gets or sets the initial <see cref="PipeStream.ReadTimeout"/>
		/// value.
		/// </summary>
		/// <value>
		/// Type: <see cref="Int32"/>
		/// <para>The initial <see cref="PipeStream.ReadTimeout"/> time, in 
		/// milliseconds, for each <see cref="PipeStream"/> in the 
		/// <see cref="PipeStreams"/> collection. A value of zero means 
		/// never time out.</para>
		/// </value>
		/// <remarks>
		/// You must set the <b>DefaultReadTimeout</b> before opening the
		/// <see cref="UsbDevice"/>. The <see cref="Open">Open</see> method
		/// will set the <see cref="PipeStream.ReadTimeout"/> property of
		/// each <see cref="PipeStream"/> to this value.
		/// <para>
		/// The default value is 100 milliseconds.</para>
		/// </remarks>
		public int DefaultReadTimeout { get; set; }

		/// <summary>
		/// Gets or sets the object that contains data about the <see cref="UsbDevice"/>.
		/// </summary>
		/// <value>
		/// Type: <see cref="Object"/>
		/// <para>An <see cref="Object"/> that contains data about the 
		/// <see cref="UsbDevice"/>. The default is <b>null</b>.</para>
		/// </value>
		/// <remarks>
		/// Any type derived from the <see cref="Object"/> class can be
		/// assigned to this property.
		/// </remarks>
		public object Tag { get; set; }

		/// <summary>
		/// Callback function used to get the string returned by <see cref="ToString">ToString</see>.
		/// </summary>
		/// <remarks>
		/// By default, <see cref="ToString">ToString</see> simply returns 
		/// the fully qualified name of the type of <see cref="UsbDevice"/>, 
		/// "WinUsbNet.UsbDevice". This default behavior can be changed by 
		/// assigning a callback function to <b>GetMyString</b>. The string
		/// returned by the callback function will be the string returned
		/// by <see cref="ToString">ToString</see>.
		/// <para>
		/// The signature for the callback function is:
		/// <code>
		/// string MyStringCallback(UsbDevice dev)
		/// </code>
		/// </para>
		/// </remarks>
		/// <example>
		/// The <see cref="Tag"/> property can be used to store the string
		/// to be returned by <see cref="ToString">ToString</see>. This can
		/// be done directly, with the <see cref="Tag"/> simply assigned the
		/// string, or indirectly, as a member of the object assigned to 
		/// <see cref="Tag"/>. This example demonstrates a callback that
		/// can be used in the direct case, using an anonymous method.
		/// <code>
		/// // dev is an instance of UsbDevice
		/// dev.GetMyString = delegate(UsbDevice device) { return (string)device.Tag; };
		/// </code>
		/// </example>
		public GetMyStringCallback GetMyString { get; set; }

		#endregion


		#region Public Methods

		/// <summary>
		/// Open the USB device for I/O.
		/// </summary>
		/// <exception cref="IOException">The <see cref="IsAttached"/>
		/// property is <b>false</b>.</exception>
		/// <exception cref="Win32Exception">An error was reported by
		/// the operating system.</exception>
		/// <remarks>
		/// If the open method succeeds, it will create the 
		/// <see cref="PipeStreams"/> collection and populate it with
		/// <see cref="PipeStream"/> objects corresponding to the
		/// endpoints on the USB device. The initial value of
		/// the <see cref="PipeStream.ReadTimeout"/> property of each
		/// <see cref="PipeStream"/> will be set to
		/// <see cref="DefaultReadTimeout"/>.
		/// <para>
		/// If the open fails, an exception will be thrown.</para>
		/// <para>
		/// USB devices are opened exclusively. That is, once a USB device
		/// has been opened and not yet closed, any additional attempt to
		/// open it (whether in the same or a different application) will fail
		/// with the <see cref="Win32Exception"/> <b>Access is denied</b>,
		/// <see cref="Win32Exception.NativeErrorCode"/> value 0x00000005.</para>
		/// </remarks>
		public void Open()
		{
			WinUsbApi.USB_INTERFACE_DESCRIPTOR descIface;
			WinUsbApi.WINUSB_PIPE_INFORMATION infoPipe;
			byte idPipe;
			PipeStream pipe;

			if (!IsAttached)
				throw new IOException(ErrDeviceNotAttached);

			try
			{
				m_hDevice = FileIO.CreateFile(
					m_strDeviceName,
					FileIO.GENERIC_WRITE | FileIO.GENERIC_READ,
					FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE,
					IntPtr.Zero,
					FileIO.OPEN_EXISTING,
					FileIO.FILE_ATTRIBUTE_NORMAL | FileIO.FILE_FLAG_OVERLAPPED,
					IntPtr.Zero);

				if (m_hDevice.IsInvalid)
					throw new Win32Exception();

				if (!WinUsbApi.WinUsb_Initialize(m_hDevice, out m_hWinUsb))
					throw new Win32Exception();

				if (!WinUsbApi.WinUsb_QueryInterfaceSettings(m_hWinUsb, 0, out descIface))
					throw new Win32Exception();

				m_arPipeStreams = new PipeStream[16];
				m_collPipeStreams = new PipeStreamCollection(m_arPipeStreams);

				// Enumerate the pipes
				for (byte i = 0; i < descIface.bNumEndpoints; i++)
				{
					if (!WinUsbApi.WinUsb_QueryPipe(m_hWinUsb, 0, i, out infoPipe))
						throw new Win32Exception();

					idPipe = (byte)(infoPipe.PipeId & WinUsbApi.PipeMask);
					if (idPipe > 15)
						continue;	// skip it

					if (m_arPipeStreams[idPipe] == null)
						m_arPipeStreams[idPipe] = new PipeStream(this, idPipe);

					pipe = m_arPipeStreams[idPipe];
					if ((infoPipe.PipeId & ~WinUsbApi.PipeMask) == WinUsbApi.ReadFlag)
					{
						pipe.m_fCanRead = true;
						pipe.m_cbReadMaxPacket = infoPipe.MaximumPacketSize;
						pipe.ReadTimeout = DefaultReadTimeout;
					}
					else
					{
						pipe.m_fCanWrite = true;
						pipe.m_cbWriteMaxPacket = infoPipe.MaximumPacketSize;
					}

					SetPipePolicyBool(infoPipe.PipeId, WinUsbApi.POLICY_TYPE.AUTO_CLEAR_STALL, true);
				}
			}
			catch
			{
				Close();
				throw;
			}
		}

		//*****************************************************************

		/// <summary>
		/// Closes the USB device.
		/// </summary>
		/// <remarks>
		/// Calling this method will set the <see cref="PipeStreams"/> property
		/// to <b>null</b>, and the <see cref="IsOpen"/> property to <b>false</b>.
		/// </remarks>
		public void Close()
		{
			if (m_hWinUsb != INVALID_USB_HANDLE)
			{
				WinUsbApi.WinUsb_Free(m_hWinUsb);
				m_hWinUsb = INVALID_USB_HANDLE;
			}

			if (m_hDevice != null)
			{
				if (!m_hDevice.IsInvalid)
					m_hDevice.Close();
				m_hDevice = null;
			}

			m_arPipeStreams = null;
			m_collPipeStreams = null;
		}

		//*****************************************************************

		/// <summary>
		/// Initiates a USB control transfer with optional data sent to the USB device.
		/// </summary>
		/// <param name="bmRequestType">The USB <i>bmRequestType</i>field.</param>
		/// <param name="bRequest">The USB <i>bRequest</i>field.</param>
		/// <param name="wValue">The USB <i>wValue</i>field.</param>
		/// <param name="wIndex">The USB <i>wIndex</i>field.</param>
		/// <param name="buffer">An array of bytes with the data to write to the 
		/// USB device. May be <b>null</b>.</param>
		/// <exception cref="IOException">The <see cref="UsbDevice"/> is not open,
		/// or the transfer did not complete.</exception>
		/// <exception cref="Win32Exception">An error was reported by
		/// the operating system.</exception>
		/// <remarks>
		/// The first four arguments of this method correspond precisely with the
		/// like-named fields of a USB Setup packet. See the USB specification
		/// for the meaning and use of these parameters.
		/// <para>
		/// The <i>wLength</i> field of the USB Setup packet is set to the 
		/// length of <paramref name="buffer"/>, or zero if <paramref name="buffer"/>
		/// is <b>null</b>.
		/// </para>
		/// </remarks>
		public void ControlWrite(byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex, byte[] buffer)
		{
			bool retval;
			uint cbWritten;
			WinUsbApi.WINUSB_SETUP_PACKET packet;

			packet.RequestType = (byte)(bmRequestType & WinUsbApi.PipeMask);	// ensure OUT transfer
			packet.Request = bRequest;
			packet.Value = wValue;
			packet.Index = wIndex;
			if (buffer == null)
				buffer = new byte[0];
			packet.Length = (ushort)buffer.Length;

			retval = WinUsbApi.WinUsb_ControlTransfer(
				UsbHandle,
				packet,
				buffer,
				(uint)buffer.Length,
				out cbWritten,
				IntPtr.Zero);

			if (!retval)
				throw new Win32Exception();

			if (cbWritten != buffer.Length)
				throw new IOException();
		}

		//*****************************************************************

		/// <summary>
		/// Initiates a USB control transfer that reads data from the USB device.
		/// </summary>
		/// <param name="bmRequestType">The USB <i>bmRequestType</i>field.</param>
		/// <param name="bRequest">The USB <i>bRequest</i>field.</param>
		/// <param name="wValue">The USB <i>wValue</i>field.</param>
		/// <param name="wIndex">The USB <i>wIndex</i>field.</param>
		/// <param name="wLength">The USB <i>wLength</i>field.</param>
		/// <returns>
		/// Type: <see cref="Byte"/>[]
		/// <para>A byte array containing the bytes read from the USB device.</para>
		/// </returns>
		/// <exception cref="IOException">The <see cref="UsbDevice"/> is not open.</exception>
		/// <exception cref="Win32Exception">An error was reported by
		/// the operating system.</exception>
		/// <remarks>
		/// The arguments of this method correspond precisely with the
		/// like-named fields of a USB Setup packet. See the USB specification
		/// for the meaning and use of these parameters.
		/// <para>
		/// The <paramref name="wLength"/> parameter is the maximum number of
		/// bytes to read from the USB device, and the USB device may send less.
		/// The length of the returned byte array is the actual number of
		/// bytes received.</para>
		/// </remarks>
		public byte[] ControlRead(byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex, ushort wLength)
		{
			byte[] arbBuf;
			byte[] arbData;
			uint cbRead;
			bool retval;
			WinUsbApi.WINUSB_SETUP_PACKET packet;

			packet.RequestType = (byte)(bmRequestType | WinUsbApi.ReadFlag);	// ensure IN transfer
			packet.Request = bRequest;
			packet.Value = wValue;
			packet.Index = wIndex;
			packet.Length = wLength;
			arbBuf = new byte[wLength];

			retval = WinUsbApi.WinUsb_ControlTransfer(
				UsbHandle,
				packet,
				arbBuf,
				(uint)arbBuf.Length,
				out cbRead,
				IntPtr.Zero);

			if (!retval)
				throw new Win32Exception();

			if (cbRead == arbBuf.Length)
				return arbBuf;

			arbData = new byte[cbRead];
			Array.Copy(arbBuf, arbData, cbRead);
			return arbData;
		}

		//*****************************************************************

		/// <summary>
		/// Returns a string that represents the <see cref="UsbDevice"/>.
		/// </summary>
		/// <returns>
		/// Type: <see cref="String"/>
		/// <para>A string that represents the <see cref="UsbDevice"/></para>
		/// </returns>
		/// <remarks>
		/// By default, this method simply returns the fully qualified name
		/// of the type of <see cref="UsbDevice"/>, "WinUsbNet.UsbDevice".
		/// This default behavior can be changed by assigning a callback
		/// function to <see cref="GetMyString"/>.
		/// </remarks>
		public override string ToString()
		{
			if (GetMyString != null)
				return GetMyString(this);

			return base.ToString();
		}
		#endregion


		#region Internal Methods

		internal void SetPipePolicyBool(int idPipe, WinUsbApi.POLICY_TYPE type, bool value)
		{
			WinUsbApi.WinUsb_SetPipePolicyBool(
				UsbHandle,
				(byte)idPipe,
				(uint)type,
				sizeof(byte),
				ref value);
		}

		internal bool GetPipePolicyBool(int idPipe, WinUsbApi.POLICY_TYPE type)
		{
			byte value;
			uint len;

			len = sizeof(byte);

			WinUsbApi.WinUsb_GetPipePolicyByte(
				UsbHandle,
				(byte)idPipe,
				(uint)type,
				ref len,
				out value);

			return value != 0;
		}

		internal void DeviceDetached()
		{
			Dispose();
			IsAttached = false;
		}

		#endregion


		#region IDisposable Members and Dispose implementation

		internal void Dispose()
		{
			Close();
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
