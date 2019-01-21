using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PatersonTech
{
	public class FormSettings
	{
		#region Constructors
		
		internal FormSettings() { }
		internal FormSettings(Form form) { SaveForm(form); }

		#endregion


		#region Public Fields - saved in Settings file

		public Point Location;
		public Size Size;
		public bool IsMaximized;

		#endregion


		#region Native Windows Interface
		
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		private struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public int showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public System.Drawing.Rectangle rcNormalPosition;
		}

		public static FormWindowState GetRestoreWindowState(Form form)
		{
			const int WPF_RESTORETOMAXIMIZED = 0x2;
			WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
			placement.length = Marshal.SizeOf(placement);
			GetWindowPlacement(form.Handle, ref placement);

			if ((placement.flags & WPF_RESTORETOMAXIMIZED) == WPF_RESTORETOMAXIMIZED)
				return FormWindowState.Maximized;
			else
				return FormWindowState.Normal;
		}

		#endregion


		#region Internal Methods

		internal void SaveForm(Form form)
		{
			FormWindowState state;
			Rectangle bounds;

			state = form.WindowState;
			bounds = state == FormWindowState.Normal ? form.Bounds : form.RestoreBounds;
			Location = bounds.Location;
			Size = bounds.Size;

			if (state == FormWindowState.Minimized)
				state = GetRestoreWindowState(form);
			IsMaximized = state == FormWindowState.Maximized;
		}

		internal bool RestoreForm(Form form)
		{
			Rectangle rectForm;
			Rectangle rectScreen;
			Screen screen;
			Screen scrForm;
			Screen scrLast;
			int pos;

			if (Size.Height == 0)
				return false;

			// Make sure saved location is visible
			rectForm = new Rectangle(Location, Size);
			scrForm = Screen.FromRectangle(rectForm);
			// scrForm is screen we'll use if not fully visible
			screen = scrForm;
			for (; ; )
			{
				rectScreen = screen.WorkingArea;
				// See if one edge of our form fits on this screen
				if (rectScreen.Left <= rectForm.Left && rectScreen.Right >= rectForm.Right)
				{
					if (rectScreen.Top <= rectForm.Top && rectScreen.Bottom > rectForm.Top)
					{
						pos = Math.Min(rectScreen.Bottom, rectForm.Bottom);
						rectForm.Height = rectForm.Bottom - pos;
						rectForm.Y = pos;
					}
					else if (rectScreen.Top < rectForm.Bottom && rectScreen.Bottom >= rectForm.Bottom)
						rectForm.Height = Math.Max(rectScreen.Top, rectForm.Top) - rectForm.Top;
				}
				else if (rectScreen.Top <= rectForm.Top && rectScreen.Bottom >= rectForm.Bottom)
				{
					if (rectScreen.Left <= rectForm.Left && rectScreen.Right > rectForm.Left)
					{
						pos = Math.Min(rectScreen.Right, rectForm.Right);
						rectForm.Width = rectForm.Right - pos;
						rectForm.X = pos;
					}
					else if (rectScreen.Left <= rectForm.Right && rectScreen.Right > rectForm.Right)
						rectForm.Width = Math.Max(rectScreen.Left, rectForm.Left) - rectForm.Left;
				}
				if (rectForm.Height == 0 || rectForm.Width == 0)
					break;
				scrLast = screen;
				screen = Screen.FromRectangle(rectForm);
				if (scrLast.Equals(screen))
				{
					// not on screen, move it
					rectScreen = scrForm.WorkingArea;
					Location.X = Math.Max(rectScreen.X, Math.Min(Location.X, rectScreen.Right - Size.Width));
					Location.Y = Math.Max(rectScreen.Y, Math.Min(Location.Y, rectScreen.Bottom - Size.Height));
					Size.Width = Math.Min(rectScreen.Width, Size.Width);
					Size.Height = Math.Min(rectScreen.Height, Size.Height);
					break;
				}
			}

			form.Location = Location;
			form.Size = Size;
			if (IsMaximized)
				form.WindowState = FormWindowState.Maximized;
			return true;
		}

		internal static bool RestoreForm(FormSettings settings, Form form)
		{
			if (settings != null)
				return settings.RestoreForm(form);
			return false;
		}

		#endregion
	}
}
