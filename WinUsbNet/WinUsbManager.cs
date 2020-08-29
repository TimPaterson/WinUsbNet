using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WinUsbNet
{
	/// <summary>
	/// The collection of matching USB devices that are currently attached.
	/// </summary>
	/// <remarks>
	/// This collection contains one <see cref="UsbDevice"/> for each
	/// USB device that is attached and whose WinUSB GUID matches the GUID of the parent
	/// <see cref="WinUsbManager"/>.
	/// </remarks>
	public sealed class UsbDeviceCollection : ReadOnlyCollection<UsbDevice>
	{
		internal UsbDeviceCollection(Collection<UsbDevice> collUsb)
			: base(collUsb)
		{
		}
	}

	/// <summary>
	/// The root class for the managed interface to WinUSB.sys.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The first step in communicating with a USB device is to use the
	/// <see cref="WinUsbManager(Guid)">WinUsbManager</see> constructor
	/// to create an instance of <b>WinUsbManager</b>. The <see cref="UsbDevices"/>
	/// collection will be populated with any USB devices with a matching 
	/// GUID that are already attached. You can subscribe to the
	/// <see cref="DeviceChange"/> event to be informed when this collection
	/// changes.</para>
	/// <para>
	/// The next step is to select a <see cref="UsbDevice"/> from the 
	/// <see cref="UsbDevices"/> collection and call its 
	/// <see cref="UsbDevice.Open">Open</see> method. A very simple
	/// application could simply use <see cref="UsbDevices"/>[0], but
	/// this may fail when multiple instances of the application are 
	/// running due to the exclusive lock on open USB devices. A better
	/// approach would be to enumerate the <see cref="UsbDevices"/> collection
	/// to determine which <see cref="UsbDevice"/> object(s) can be
	/// successfully opened.</para>
	/// <para>
	/// I/O with the USB device is performed using a <see cref="PipeStream"/>
	/// from the <see cref="UsbDevice.PipeStreams"/> collection of the <see cref="UsbDevice"/>.
	/// A <see cref="PipeStream"/> represents a communications pipe with a
	/// specific USB endpoint.</para>
	/// <para>
	/// After I/O with the USB device is completed, call the 
	/// <see cref="UsbDevice.Close">Close</see> method on the <see cref="UsbDevice"/>.
	/// Finally, call the <see cref="Dispose()">Dispose</see> method
	/// when done using <b>WinUsbManager</b>.
	/// </para>
	/// </remarks>
	/// <example>
	/// This example assumes a <see cref="System.Windows.Forms.Form"/> named <b>Form1</b> with a 
	/// <see cref="System.Windows.Forms.TextBox"/> named <b>txtGuid</b> and a <see cref="System.Windows.Forms.ListBox"/>
	/// named <b>lstDevices</b>. It creates a <see cref="WinUsbManager"/> 
	/// using the GUID in the text box, and then maintains a list of
	/// currently attached matching USB devices in the list box. The devices
	/// are identified by their manufacturer, product name, and serial
	/// number as present in the USB device. Double-clicking on a USB device
	/// in the list box will trigger some I/O transfers.
	/// <code source="..\WinUsbExample\Form1.cs" />
	/// </example>
	public class WinUsbManager : IDisposable
	{
		#region Constructor & Destructor
		
		/// <summary>
		/// Initializes a new instance of the <see cref="WinUsbManager"/> class.
		/// </summary>
		/// <param name="guid">GUID specified in the device driver installation file (INF).</param>
		/// <remarks>
		/// <para>
		/// Each USB device that uses the generic USB device driver WinUSB.sys
		/// is assigned a GUID in the device driver installation file (INF).
		/// To create an instance of <see cref="WinUsbManager"/>, this GUID
		/// must be passed to the constructor.</para>
		/// <para>
		/// One instance of <see cref="WinUsbManager"/> handles all attached
		/// USB devices with the assigned GUID. To handle another USB device with 
		/// a different GUID, another instance of <see cref="WinUsbManager"/>
		/// must be created.</para>
		/// </remarks>
		public WinUsbManager(Guid guid)
		{
			// Set up notification of attach/remove
			m_ctlNotify = new DeviceNotifyControl();
			m_hNotify = DeviceManagement.RegisterForDeviceNotifications(m_ctlNotify.Handle, guid);
			if (m_hNotify != IntPtr.Zero)
			{
				m_ctlNotify.DeviceChangeEvent += DeviceChangeEventHandler;

				m_collUsbRW = new Collection<UsbDevice>();
				m_collUsbDevice = new UsbDeviceCollection(m_collUsbRW);

				// See if any matching devices are already attached
				foreach (string strDeviceName in DeviceManagement.GetDeviceNames(guid))
					m_collUsbRW.Add(new UsbDevice(strDeviceName));
			}
		}

		/// <summary>
		/// Allows a <see cref="WinUsbManager"/> object to free resources 
		/// before it is reclaimed by garbage collection.
		/// </summary>
		/// <seealso cref="Object.Finalize"/>
		~WinUsbManager()
		{
			Dispose(false);
		}

		#endregion


		#region Private Fields

		readonly DeviceNotifyControl m_ctlNotify;
		IntPtr m_hNotify;
		Collection<UsbDevice> m_collUsbRW;
		UsbDeviceCollection m_collUsbDevice;

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the collection of matching USB devices that are currently attached.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This collection contains one <see cref="UsbDevice"/> for each
		/// USB device that is attached and whose WinUSB GUID matches the GUID of the parent
		/// <see cref="WinUsbManager"/>.</para>
		/// <para>
		/// When <see cref="WinUsbManager"/> is created, this collection is immediately
		/// populated with the USB devices that are currently attached, if any. As devices
		/// are attached and detached, the collection is constantly updated. Because of this,
		/// the index of a given member may change over time.</para>
		/// </remarks>
		/// <seealso cref="UsbDeviceCollection"/>
		public UsbDeviceCollection UsbDevices
		{
			get { return m_collUsbDevice; }
		}

		#endregion


		#region Events

		/// <summary>
		/// Occurs when a matching USB device is attached or detached.
		/// </summary>
		/// <example>
		/// This example demonstrates using the <see cref="DeviceChangeEventArgs.IsAttach"/>
		/// property of <see cref="DeviceChangeEventArgs"/> to determine if 
		/// the event is fired for attach or detach.
		/// <code>
		/// void DeviceChange(object sender, DeviceChangeEventArgs e)
		/// {
		///     if (e.IsAttach)
		///         GetSerialNumber(e.UsbDevice);
		///     else
		///         lstDevices.Items.Remove(e.UsbDevice);
		/// }
		/// </code>
		/// </example>
		public event EventHandler<DeviceChangeEventArgs> DeviceChange;

		#endregion


		#region Event Handlers

		void DeviceChangeEventHandler(object sender, DeviceChangeEventArgs e)
		{
			Debug.WriteLine("Device " + (e.IsAttach ? "attached: " : "detached: ") + e.DeviceName);

			if (e.IsAttach)
			{
				e.UsbDevice = new UsbDevice(e.DeviceName);
				m_collUsbRW.Add(e.UsbDevice);

				if (DeviceChange != null)
					DeviceChange(this, e);
			}
			else
			{
				foreach (UsbDevice dev in m_collUsbRW)
				{
					if (string.Compare(e.DeviceName, dev.DeviceName, true) == 0)
					{
						dev.DeviceDetached();
						e.UsbDevice = dev;
						m_collUsbRW.Remove(dev);

						if (DeviceChange != null)
							DeviceChange(this, e);

						break;	// foreach no longer valid
					}
				}
			}
		}

		#endregion


		#region IDisposable Members and Dispose implementation

		/// <summary>
		/// Releases all resources used by the <see cref="WinUsbManager"/> object.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="WinUsbManager"/>,
		/// and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing"><b>true</b> to release both managed and 
		/// unmanaged resources; <b>false</b> to release only unmanaged resources.</param>
		/// <remarks>
		/// This method is called by the public <see cref="Dispose()">Dispose</see> 
		/// method and the <see cref="Finalize">Finalize</see> method. 
		/// <see cref="Dispose()">Dispose</see> invokes the 
		/// protected <b>Dispose</b> method with the <paramref name="disposing"/>
		/// parameter set to <b>true</b>. <see cref="Finalize">Finalize</see> invokes 
		/// <b>Dispose</b> with <paramref name="disposing"/> set to <b>false</b>.
		/// </remarks>
		protected virtual void Dispose(bool disposing)
		{
			if (m_hNotify != IntPtr.Zero)
			{
				DeviceManagement.UnregisterDeviceNotification(m_hNotify);
				m_hNotify = IntPtr.Zero;

				if (disposing)
				{
					foreach (UsbDevice dev in m_collUsbRW)
						dev.Dispose();
				}
			}
		}

		#endregion
	}
}
