using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;


namespace VisualCrypt.Desktop.Shared.Controls
{
	public class AppDialog : Window
	{
		static AppDialog()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (AppDialog),
				new FrameworkPropertyMetadata(typeof (AppDialog)));
		}

		public AppDialog()
		{
			WindowStyle = WindowStyle.None;
		}

		Rectangle _moveRectangle;
		Button _restoreButton;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var minimizeButton = (Button) GetTemplateChild("minimizeButton");
			if (minimizeButton != null)
				minimizeButton.Click += Minimize_Click;

			_restoreButton = (Button) GetTemplateChild("restoreButton");
			if (_restoreButton != null)
				_restoreButton.Click += Restore_Click;


			var closeButton = (Button) GetTemplateChild("closeButton");
			if (closeButton != null)
				closeButton.Click += Close_Click;

			_moveRectangle = (Rectangle) GetTemplateChild("moveRectangle");

			if (_moveRectangle != null)
			{
				_moveRectangle.MouseDown += MoveRectangle_MouseDown;
			}

			var resizeGrid = (Panel) GetTemplateChild("resizeGrid");
			if (resizeGrid != null)
				foreach (UIElement rectangle in resizeGrid.Children)
				{
					rectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
					rectangle.MouseMove += ResizeRectangle_MouseMove;
					rectangle.MouseLeave += ResizeRectangle_MouseLeave;
				}
		}


		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			SourceInitialized += OnSourceInitialized;
			PreviewKeyDown += Window_PreviewKeyDown;
		}

		#region Event handlers

		void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			var isAltPressed = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
			if (e.Key == Key.System && Keyboard.IsKeyDown(Key.Space) && isAltPressed)
			{
				e.Handled = true;
				CenterOnPrimaryScreenWithDefaults();
			}
		}

		void CenterOnPrimaryScreenWithDefaults()
		{
			Left = (SystemParameters.WorkArea.Width/2) - (Width/2);
			Top = (SystemParameters.WorkArea.Height/2) - (Height/2);
			WindowState = WindowState.Normal;
			SetCanResize(true);
		}


		void Minimize_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		void Restore_Click(object sender, RoutedEventArgs e)
		{
			SwitchWindowState();
		}

		void Close_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		void MoveRectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
				return;

			var mPoint = Mouse.GetPosition(this);
			IntPtr windowHandle = new WindowInteropHelper(this).Handle;
			ReleaseCapture();
			var wpfPoint = PointToScreen(mPoint);
			var x = Convert.ToInt16(wpfPoint.X);
			var y = Convert.ToInt16(wpfPoint.Y);
			var lParam = (int) (uint) x | (y << 16);

			SendMessage(windowHandle, WmNclbuttondown, HtCaption, lParam);
			SetCanResize(true);

			var canResize = ResizeMode == ResizeMode.CanResizeWithGrip || ResizeMode == ResizeMode.CanResize;

			if (e.ClickCount == 2 && canResize)
			{
				SwitchWindowState();
			}
		}


		void ResizeRectangle_MouseLeave(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
				Cursor = Cursors.Arrow;
		}

		void ResizeRectangle_MouseMove(Object sender, MouseEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
				return;

			var rectangle = sender as Rectangle;
			if (rectangle != null)
				switch (rectangle.Name)
				{
					case "top":
						Cursor = Cursors.SizeNS;
						break;
					case "bottom":
						Cursor = Cursors.SizeNS;
						break;
					case "left":
						Cursor = Cursors.SizeWE;
						break;
					case "right":
						Cursor = Cursors.SizeWE;
						break;
					case "topLeft":
						Cursor = Cursors.SizeNWSE;
						break;
					case "topRight":
						Cursor = Cursors.SizeNESW;
						break;
					case "bottomLeft":
						Cursor = Cursors.SizeNESW;
						break;
					case "bottomRight":
						Cursor = Cursors.SizeNWSE;
						break;
				}
		}

		void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
				return;
			var rectangle = sender as Rectangle;
			if (rectangle != null)
				switch (rectangle.Name)
				{
					case "top":
						Cursor = Cursors.SizeNS;
						ResizeWindow(ResizeDirection.Top);
						break;
					case "bottom":
						Cursor = Cursors.SizeNS;
						ResizeWindow(ResizeDirection.Bottom);
						break;
					case "left":
						Cursor = Cursors.SizeWE;
						ResizeWindow(ResizeDirection.Left);
						break;
					case "right":
						Cursor = Cursors.SizeWE;
						ResizeWindow(ResizeDirection.Right);
						break;
					case "topLeft":
						Cursor = Cursors.SizeNWSE;
						ResizeWindow(ResizeDirection.TopLeft);
						break;
					case "topRight":
						Cursor = Cursors.SizeNESW;
						ResizeWindow(ResizeDirection.TopRight);
						break;
					case "bottomLeft":
						Cursor = Cursors.SizeNESW;
						ResizeWindow(ResizeDirection.BottomLeft);
						break;
					case "bottomRight":
						Cursor = Cursors.SizeNWSE;
						ResizeWindow(ResizeDirection.BottomRight);
						break;
				}
		}


		enum ResizeDirection
		{
			Left = 1,
			Right = 2,
			Top = 3,
			TopLeft = 4,
			TopRight = 5,
			Bottom = 6,
			BottomLeft = 7,
			BottomRight = 8,
		}

		void SwitchWindowState()
		{
			switch (WindowState)
			{
				case WindowState.Normal:
				{
					// Maximize
					_restoreButton.Content = 2;
					WindowState = WindowState.Maximized;
					SetCanResize(false);
					break;
				}
				case WindowState.Maximized:
				{
					// Restore to normal size.
					_restoreButton.Content = 1;
					WindowState = WindowState.Normal;
					SetCanResize(false);
					break;
				}
			}
		}


		void SetCanResize(bool canResize)
		{
			var resizeGrid = (Panel) GetTemplateChild("resizeGrid");
			if (resizeGrid != null)
				foreach (UIElement rectangle in resizeGrid.Children)
				{
					rectangle.IsHitTestVisible = canResize;
				}
		}

		#endregion

		#region interop

		const int WmNclbuttondown = 0x00A1;
		const int HtCaption = 0x2;
		HwndSource _hwndSource;

		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetCursorPos(out POINT lpPoint);


		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

		[DllImport("user32.dll")]
		static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);


		void ResizeWindow(ResizeDirection direction)
		{
			SendMessage(_hwndSource.Handle, 0x112, (IntPtr) (61440 + direction), IntPtr.Zero);
		}

		void OnSourceInitialized(object sender, EventArgs e)
		{
			_hwndSource = (HwndSource) PresentationSource.FromVisual(this);
			if (_hwndSource != null)
				_hwndSource.AddHook(WindowProc);
		}

		static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case 0x0024:
					WmGetMinMaxInfo(lParam);
					break;
			}

			return IntPtr.Zero;
		}

		static void WmGetMinMaxInfo(IntPtr lParam)
		{
			POINT lMousePosition;
			GetCursorPos(out lMousePosition);

			IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
			var lPrimaryScreenInfo = new MONITORINFO();
			if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
			{
				return;
			}

			IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

			var lMmi = (MINMAXINFO) Marshal.PtrToStructure(lParam, typeof (MINMAXINFO));

			if (lPrimaryScreen.Equals(lCurrentScreen))
			{
				lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
				lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
				lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
				lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
			}
			else
			{
				lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
				lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
				lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
				lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
			}

			Marshal.StructureToPtr(lMmi, lParam, true);
		}

		// ReSharper disable InconsistentNaming

		enum MonitorOptions : uint
		{
			MONITOR_DEFAULTTOPRIMARY = 0x00000001,
			MONITOR_DEFAULTTONEAREST = 0x00000002
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			public POINT ptReserved;
			public POINT ptMaxSize;
			public POINT ptMaxPosition;
			public POINT ptMinTrackSize;
			public POINT ptMaxTrackSize;
		};


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class MONITORINFO
		{
			public int cbSize = Marshal.SizeOf(typeof (MONITORINFO));
			public RECT rcMonitor = new RECT();
			public RECT rcWork = new RECT();
			public int dwFlags = 0;
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public readonly int Left, Top, Right, Bottom;

			public RECT(int left, int top, int right, int bottom)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}
		}
	}

	#endregion
}