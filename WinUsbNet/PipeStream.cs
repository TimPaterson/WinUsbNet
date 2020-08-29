using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace WinUsbNet
{
	/// <summary>
	/// Provides a <see cref="Stream"/> for transferring data through a USB pipe.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A USB device provides one or more endpoints for communicating with
	/// the host computer. The logical connection between the endpoint and the host
	/// is a USB pipe. The <b>PipeStream</b> provides methods for transferring data
	/// over this pipe.
	/// </para>
	/// <para>
	/// You do not create a <b>PipeStream</b> directly. A <b>PipeStream</b> is
	/// automatically created for each endpoint of the USB device when the 
	/// <see cref="UsbDevice.Open">Open</see> method on the <see cref="UsbDevice"/> 
	/// is called. You access a <b>PipeStream</b> through the 
	/// <see cref="UsbDevice.PipeStreams"/> collection of the <see cref="UsbDevice"/>.
	/// Do not use the <b>Close</b> or <b>Dispose</b> methods
	/// on the <b>PipeStream</b>. The <b>PipeStream</b> is closed automatically
	/// when the USB device is closed or disconnected.
	/// </para>
	/// <para>
	/// Each endpoint, and its corresponding pipe, provides one-way
	/// communication. It is possible (but rare) that two endpoints with opposite
	/// direction of data transfer will share the same endpoint number. In this 
	/// case, a single <b>PipeStream</b> is used for the two pipes.</para>
	/// <para>
	/// The <see cref="CanRead"/> and <see cref="CanWrite"/> properties identify
	/// the direction of data transfor for the pipe. The <see cref="Read">Read</see> and
	/// <see cref="Write">Write</see> methods are used to transfer an array of bytes, while
	/// the <see cref="ReadByte">ReadByte</see> and <see cref="WriteByte">WriteByte</see>
	/// methods are used to transfer a single byte at a time. Transfers of more 
	/// structured data can be accomplished using the <b>PipeStream</b> with classes 
	/// such as <see cref="BinaryReader"/>, and <see cref="BinaryWriter"/> with
	/// <see cref="BufferedStream"/>.</para>
	/// <para>
	/// All transfers are synchronous and blocking. If you request to read more
	/// data than the USB device has ready, the <see cref="ReadTimeout"/> property
	/// will determine how long the read will block waiting for data. Write
	/// transfers are normally very fast so the <b>WriteTimeout</b>
	/// property is not used and will throw an exception.</para>
	/// <para>
	/// USB transfers are carried in packets of a maximum size specified by the
	/// USB device. Transfers larger than the packet size will automatically use
	/// multiple packets. A packet of less than maximum size (short packet) can 
	/// be used to indicate that a transfer is complete. The use of short packets
	/// is entirely up to the communication protocol that the device and host are using.
	/// If the length of data transfer is otherwise implicitly or explicity established,
	/// there is no need to use short packets.</para>
	/// <para>
	/// If the USB device uses a short packet to indicate that it has no more data to
	/// send, set the <see cref="ReadUseShortPacket"/> property to <b>true</b>.
	/// Then the <see cref="Read">Read</see> or <see cref="ReadByte">ReadByte</see>
	/// methods will terminate if they receive a short packet. If the 
	/// <see cref="ReadUseShortPacket"/> property is <b>false</b>, the
	/// <see cref="Read">Read</see> or <see cref="ReadByte">ReadByte</see> methods will 
	/// ignore short packets and continue waiting for data until the number of bytes 
	/// requested is received or the <see cref="ReadTimeout"/> limit is reached.</para>
	/// <para>
	/// If the USB device accepts a short packet as an indicator that the host has no
	/// more data to send, set the <see cref="WriteUseShortPacket"/>
	/// property to <b>true</b>. This will ensure that the <see cref="Write">Write</see>
	/// method will always end with a short packet, appending a zero-length packet
	/// if necessary (if the transfer is a multiple of the maximum packet size).
	/// Note that the <see cref="Write">Write</see> and <see cref="WriteByte">WriteByte</see> methods
	/// perform no buffering. <see cref="WriteByte">WriteByte</see> will always send a 1-byte
	/// (short) packet.</para>
	/// <para>
	/// The <b>Length</b> and <b>Position</b> 
	/// properties and the <b>Seek</b> and <b>SetLength</b> methods are 
	/// not meaningful and will throw an exception if used.</para>
	/// </remarks>
	public sealed class PipeStream : Stream
	{
		#region Constructor

		internal PipeStream(UsbDevice device, byte idPipe)
		{
			m_Device = device;
			m_idPipe = idPipe;
		}

		#endregion


		#region Internal Fields
		
		internal bool m_fCanRead;
		internal bool m_fCanWrite;
		internal int m_cbReadMaxPacket;
		internal int m_cbWriteMaxPacket;

		#endregion


		#region Private Fields

		UsbDevice m_Device;
		byte m_idPipe;

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets or sets a value indicating if a short packet should be used to signal 
		/// to the USB device that the host has no more data to send.
		/// </summary>
		/// <value>
		/// <para><b>true</b> if a short packet will be used; otherwise <b>false</b>.</para>
		/// </value>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support writing.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This property only has an affect when the number of bytes transferred by
		/// the <see cref="Write">Write</see> method happens to equal a multiple of
		/// <see cref="WriteMaxPacketSize"/>, the maximum packet size for the USB 
		/// endpoint. In that case, if the <b>WriteUseShortPacket</b> property is 
		/// <b>true</b>, then a zero-length packet will be sent after the data.</para>
		/// <para>
		/// If the <b>WriteUseShortPacket</b> property is <b>false</b>, then a
		/// zero-length packet will never be added.</para>
		/// </remarks>
		public bool WriteUseShortPacket
		{
			get 
			{ 
				if (!m_fCanWrite)
					throw new NotSupportedException();

				return m_Device.GetPipePolicyBool(m_idPipe | WinUsbApi.WriteFlag, WinUsbApi.POLICY_TYPE.SHORT_PACKET_TERMINATE); 
			}
			set 
			{ 
				if (!m_fCanWrite)
					throw new NotSupportedException();

				m_Device.SetPipePolicyBool(m_idPipe | WinUsbApi.WriteFlag, WinUsbApi.POLICY_TYPE.SHORT_PACKET_TERMINATE, value); 
			}
		}

		/// <summary>
		/// Gets or sets a value indicating if a short packet from the USB device
		/// should be used to terminate a read transfer.
		/// </summary>
		/// <value>
		/// <para><b>true</b> if a short packet will terminate a read transfer; otherwise <b>false</b>.</para>
		/// </value>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support reading.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If the <b>ReadUseShortPacket</b> property is <b>true</b>, then a short
		/// packet received from the USB device will terminate a <see cref="Read">Read</see>
		/// or <see cref="ReadByte">ReadByte</see> in progress.</para>
		/// <para>
		/// If the <b>ReadUseShortPacket</b> property is <b>false</b>, then a
		/// short packet will not terminate the read transfer. The transfer will
		/// continue until the number of bytes requested has been received or
		/// the time limit specifed by <see cref="ReadTimeout"/> has been reached.
		/// </para>
		/// </remarks>
		public bool ReadUseShortPacket
		{
			get 
			{ 
				if (!m_fCanRead)
					throw new NotSupportedException();

				return !m_Device.GetPipePolicyBool(m_idPipe | WinUsbApi.ReadFlag, WinUsbApi.POLICY_TYPE.IGNORE_SHORT_PACKETS); 
			}
			set 
			{ 
				if (!m_fCanRead)
					throw new NotSupportedException();

				m_Device.SetPipePolicyBool(m_idPipe | WinUsbApi.ReadFlag, WinUsbApi.POLICY_TYPE.IGNORE_SHORT_PACKETS, !value); 
			}
		}

		/// <summary>
		/// Gets the maximum size, in bytes, of the packets that are received on the pipe.
		/// </summary>
		/// <value>
		/// <para>The maximum size, in bytes, of the packets that are received on the pipe.</para>
		/// </value>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support reading.
		/// </exception>
		public int ReadMaxPacketSize
		{
			get 
			{ 
				if (!m_fCanRead)
					throw new NotSupportedException();

				return m_cbReadMaxPacket;
			}
		}

		/// <summary>
		/// Gets the maximum size, in bytes, of the packets that are transmitted on the pipe.
		/// </summary>
		/// <value>
		/// <para>The maximum size, in bytes, of the packets that are transmitted on the pipe.</para>
		/// </value>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support writing.
		/// </exception>
		public int WriteMaxPacketSize
		{
			get 
			{ 
				if (!m_fCanWrite)
					throw new NotSupportedException();

				return m_cbWriteMaxPacket;
			}
		}

		/// <summary>
		/// Gets the USB endpoint number for the <see cref="PipeStream"/>.
		/// </summary>
		/// <value>
		/// <para>The USB endpoint number, in the range 0 - 15.</para>
		/// </value>
		/// <remarks>
		/// The <b>PipeId</b> is also the index into the <see cref="UsbDevice.PipeStreams"/>
		/// collection of this <see cref="PipeStream"/>.
		/// </remarks>
		public int PipeId
		{
			get { return m_idPipe; }
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Clears the read buffer for the <see cref="PipeStream"/>.
		/// </summary>
		/// <exception cref="IOException">The <see cref="UsbDevice"/> is not open.</exception>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support reading.
		/// </exception>
		/// <remarks>
		/// Data received from the USB device that has not yet been read with
		/// the <see cref="Read">Read</see> or <see cref="ReadByte">ReadByte</see>
		/// methods is stored in an internal buffer. The <b>ClearReadBuffer</b> method
		/// clears the buffer, discarding any unread data.
		/// </remarks>
		public void ClearReadBuffer()
		{
			if (!m_fCanRead)
				throw new NotSupportedException();

			WinUsbApi.WinUsb_FlushPipe(m_Device.UsbHandle, (byte)(m_idPipe | WinUsbApi.ReadFlag));
		}

		#endregion


		#region Stream Implementation

		/// <summary>
		/// Gets or sets the time, in milliseconds, to wait for a read operation to complete.
		/// </summary>
		/// <value>
		/// <para>The time, in milliseconds, to wait for a read operation to complete.
		/// A value of zero means never time out.</para>
		/// </value>
		/// <exception cref="IOException">The <see cref="UsbDevice"/> is not open.</exception>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support reading.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The initial value of <b>ReadTimeout</b> is set to the value of the
		/// <see cref="UsbDevice.DefaultReadTimeout"/> property on the parent
		/// <see cref="UsbDevice"/> at the time its <see cref="UsbDevice.Open">
		/// Open</see> method is called.</para>
		/// <para>
		/// If a read operation times out, <see cref="Win32Exception"/> is thrown
		/// with <see cref="Win32Exception.NativeErrorCode"/> set to 0x00000079.</para>
		/// </remarks>
		public override int ReadTimeout
		{
			get
			{
				uint timeout;
				uint len = sizeof(uint);

				if (!m_fCanRead)
					throw new NotSupportedException();

				WinUsbApi.WinUsb_GetPipePolicyTimeout(
					m_Device.UsbHandle,
					(byte)(m_idPipe | WinUsbApi.ReadFlag),
					(uint)WinUsbApi.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT,
					ref len,
					out timeout);

				return (int)timeout;
			}

			set
			{
				uint timeout = (uint)value;

				if (!m_fCanRead)
					throw new NotSupportedException();

				WinUsbApi.WinUsb_SetPipePolicyTimeout(
					m_Device.UsbHandle, 
					(byte)(m_idPipe | WinUsbApi.ReadFlag), 
					(uint)WinUsbApi.POLICY_TYPE.PIPE_TRANSFER_TIMEOUT,
					sizeof(uint),
					ref timeout);
			}
		}

		/// <summary>
		/// Gets a value that indicates if the <see cref="PipeStream"/> can time out.
		/// </summary>
		/// <value>
		/// <para><b>true</b> if the <see cref="PipeStream"/> can read from the
		/// USB device, otherwise <b>false</b>.</para>
		/// </value>
		public override bool CanTimeout
		{
			get { return m_fCanRead; }
		}

		/// <summary>
		/// Gets a value that indicates if the <see cref="PipeStream"/> can read from the USB device.
		/// </summary>
		/// <value>
		/// <para><b>true</b> if the <see cref="PipeStream"/> can read from the
		/// USB device, otherwise <b>false</b>.</para>
		/// </value>
		public override bool CanRead
		{
			get { return m_fCanRead; }
		}

		/// <summary>
		/// Gets the value <b>false</b>, indicating the <see cref="PipeStream"/> does not support seeking.
		/// </summary>
		/// <value>
		/// <para>Always return <b>false</b>.</para>
		/// </value>
		public override bool CanSeek
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value that indicates if the <see cref="PipeStream"/> can write to the USB device.
		/// </summary>
		/// <value>
		/// <para><b>true</b> if the <see cref="PipeStream"/> can write to the
		/// USB device, otherwise <b>false</b>.</para>
		/// </value>
		public override bool CanWrite
		{
			get { return m_fCanWrite; }
		}

		/// <summary>
		/// This method does nothing.
		/// </summary>
		/// <remarks>
		/// Because write transfers are not buffered, there are no buffers to flush
		/// and this method does nothing.
		/// </remarks>
		public override void Flush()
		{
		}

		/// <summary>
		/// Attempts to read a specified number of bytes from the USB device.
		/// </summary>
		/// <param name="buffer">An array of bytes to receive the data read
		/// from the USB device.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at  
		/// which to begin storing the data read from the USB device.</param>
		/// <param name="count">The maximum number of bytes to read from the
		/// USB device.</param>
		/// <returns>
		/// <para>The total number of bytes read into <paramref name="buffer"/>. This can be
		/// less than the number of bytes requested (including zero) if a short packet is received and the 
		/// <see cref="ReadUseShortPacket"/> property is set to <b>true</b>.</para>
		/// </returns>
		/// <exception cref="IOException">The <see cref="UsbDevice"/> is not open.</exception>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support reading.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <b>null</b>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or 
		/// <paramref name="count"/> is negative.</exception>
		/// <exception cref="ArgumentException">The sum of <paramref name="offset"/> and 
		/// <paramref name="count"/> is larger than the buffer length.</exception>
		/// <exception cref="Win32Exception">An error was reported by
		/// the operating system. If <see cref="Win32Exception.NativeErrorCode"/> is 0x00000079,
		/// the operation timed out.</exception>
		/// <remarks>
		/// <para>
		/// If the USB device returns more data than requested, the excess
		/// data will be stored in an internal buffer. Future calls to
		/// <see cref="Read">Read</see> or <see cref="ReadByte">ReadByte</see>
		/// will read from the internal buffer until it is exhausted.</para>
		/// <para>
		/// If the <see cref="ReadUseShortPacket"/> property is <b>true</b>, then a short
		/// packet received from the USB device will terminate the read transfer. The number
		/// of bytes received to that point will be returned.</para>
		/// <para>
		/// If the <see cref="ReadUseShortPacket"/> property is <b>false</b>, then a
		/// short packet will not terminate the read transfer. The transfer will
		/// continue until the number of bytes requested has been received or
		/// the time limit specifed by <see cref="ReadTimeout"/> has been reached.
		/// </para>
		/// </remarks>
		/// <seealso cref="ReadByte">ReadByte</seealso>
		/// <seealso cref="Write">Write</seealso>
		public override int Read(byte[] buffer, int offset, int count)
		{
			GCHandle gch;
			uint cbRead;
			bool retval;
			IntPtr pBuf;

			if (!m_fCanRead)
				throw new NotSupportedException();

			if (buffer == null)
				throw new ArgumentNullException("buffer");

			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count");

			if (offset + count > buffer.Length)
				throw new ArgumentException();

			gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			pBuf = gch.AddrOfPinnedObject();
			pBuf = (IntPtr)(pBuf.ToInt64() + offset);

			retval = WinUsbApi.WinUsb_ReadPipe(
				m_Device.UsbHandle, 
				(byte)(m_idPipe | WinUsbApi.ReadFlag), 
				pBuf, 
				(uint)count, 
				out cbRead, 
				IntPtr.Zero);

			gch.Free();

			if (!retval)
				throw new Win32Exception();

			return (int)cbRead;
		}

		/// <summary>
		/// Attempts to read one byte from the USB device.
		/// </summary>
		/// <returns>
		/// <para>The unsigned byte cast to an Int32, or -1 if a short packet is received and the 
		/// <see cref="ReadUseShortPacket"/> property is set to <b>true</b>.</para>
		/// </returns>
		/// <exception cref="IOException">The <see cref="UsbDevice"/> is not open.</exception>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support reading.</exception>
		/// <exception cref="Win32Exception">An error was reported by
		/// the operating system. If <see cref="Win32Exception.NativeErrorCode"/> is 0x00000079,
		/// the operation timed out.</exception>
		/// <remarks>
		/// If the USB device returns more data than requested, the excess
		/// data will be stored in an internal buffer. Future calls to
		/// <see cref="Read">Read</see> or <see cref="ReadByte">ReadByte</see>
		/// will read from the internal buffer until it is exhausted.
		/// </remarks>
		/// <seealso cref="Read"/>
		/// <seealso cref="WriteByte"/>
		public override int ReadByte()
		{
			byte bData;
			uint cbRead;
			bool retval;

			if (!m_fCanRead)
				throw new NotSupportedException();

			retval = WinUsbApi.WinUsb_ReadPipeByte(
				m_Device.UsbHandle,
				(byte)(m_idPipe | WinUsbApi.ReadFlag),
				out bData,
				1,
				out cbRead,
				IntPtr.Zero);

			if (!retval)
				throw new Win32Exception();

			if (cbRead == 0)
				return -1;

			return bData;
		}

		/// <summary>
		/// Writes a specified number of bytes to the USB device.
		/// </summary>
		/// <param name="buffer">An array of bytes containing the data to
		/// write to the USB device.</param>
		/// <param name="offset">The zero-based offset in the <paramref name="buffer"/>
		/// at which to begin sending data to the USB device.</param>
		/// <param name="count">The number of bytes to write to the 
		/// USB device.</param>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support writing.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <b>null</b>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or 
		/// <paramref name="count"/> is negative.</exception>
		/// <exception cref="ArgumentException">The sum of <paramref name="offset"/> and 
		/// <paramref name="count"/> is larger than the buffer length.</exception>
		/// <exception cref="Win32Exception">An error was reported by
		/// the operating system.</exception>
		/// <exception cref="IOException">The <see cref="UsbDevice"/> is not open,
		/// or the transfer did not complete.</exception>
		/// <remarks>
		/// If the number of bytes written to the USB device is
		/// a multiple of the maximum packet size, and the <see cref="WriteUseShortPacket"/>
		/// property is set to <b>true</b>, then a zero-length packet will be
		/// sent after the data.
		/// </remarks>
		/// <seealso cref="Read">Read</seealso>
		/// <seealso cref="WriteByte">WriteByte</seealso>
		public override void Write(byte[] buffer, int offset, int count)
		{
			GCHandle gch;
			uint cbWritten;
			bool retval;
			IntPtr pBuf;

			if (!m_fCanWrite)
				throw new NotSupportedException();

			if (buffer == null)
				throw new ArgumentNullException("buffer");

			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count");

			if (offset + count > buffer.Length)
				throw new ArgumentException();

			gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			pBuf = gch.AddrOfPinnedObject();
			pBuf = (IntPtr)(pBuf.ToInt64() + offset);

			retval = WinUsbApi.WinUsb_WritePipe(
				m_Device.UsbHandle,
				(byte)(m_idPipe | WinUsbApi.WriteFlag),
				pBuf,
				(uint)count,
				out cbWritten,
				IntPtr.Zero);

			gch.Free();

			if (!retval)
				throw new Win32Exception();

			if (cbWritten != count)
				throw new IOException();
		}

		/// <summary>
		/// Writes one byte to the USB device.
		/// </summary>
		/// <param name="value">The byte to write.</param>
		/// <exception cref="NotSupportedException">The <see cref="PipeStream"/>
		/// does not support writing.</exception>
		/// <exception cref="Win32Exception">An error was reported by
		/// the operating system.</exception>
		/// <exception cref="IOException">The <see cref="UsbDevice"/> is not open,
		/// or the transfer did not complete.</exception>
		/// <remarks>
		/// Data sent to the USB device is not buffered, so this method will
		/// always send a 1-byte packet.
		/// </remarks>
		/// <seealso cref="ReadByte">ReadByte</seealso>
		/// <seealso cref="Write">Write</seealso>
		public override void WriteByte(byte value)
		{
			uint cbWritten;
			bool retval;

			if (!m_fCanWrite)
				throw new NotSupportedException();

			retval = WinUsbApi.WinUsb_WritePipeByte(
				m_Device.UsbHandle,
				(byte)(m_idPipe | WinUsbApi.WriteFlag),
				ref value,
				1,
				out cbWritten,
				IntPtr.Zero);

			if (!retval)
				throw new Win32Exception();

			if (cbWritten != 1)
				throw new IOException();
		}

		#endregion


		#region Unimplemented Stream Methods

		/// <summary>
		/// Throws <see cref="NotSupportedException"/>.
		/// </summary>
		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Throws <see cref="NotSupportedException"/>.
		/// </summary>
		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Throws <see cref="NotSupportedException"/>.
		/// </summary>
		public override int WriteTimeout
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Throws <see cref="NotSupportedException"/>.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns>Throws <see cref="NotSupportedException"/></returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Throws <see cref="NotSupportedException"/>.
		/// </summary>
		/// <param name="value"></param>
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
