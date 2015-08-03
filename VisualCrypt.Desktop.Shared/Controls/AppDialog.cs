using System;
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

		HwndSource _hwndSource;
		Rectangle _moveRectangle;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			// ReSharper disable PossibleNullReferenceException, this should fail fast
			var closeButton = (Button) GetTemplateChild("closeButton");

			closeButton.Click += Close_Click;

			_moveRectangle = (Rectangle) GetTemplateChild("moveRectangle");
			_moveRectangle.MouseDown += MoveWindow;

			var resizeGrid = (Panel) GetTemplateChild("resizeGrid");
			foreach (UIElement rectangle in resizeGrid.Children)
			{
				rectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
				rectangle.MouseMove += ResizeRectangle_MouseMove;
				rectangle.MouseLeave += ResizeRectangle_MouseLeave;
			}
			// ReSharper restore PossibleNullReferenceException
		}


		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			SourceInitialized += delegate { _hwndSource = (HwndSource) PresentationSource.FromVisual(this); };
			PreviewKeyDown += CustomizeAltSpace;
			WindowStyle = WindowStyle.None;
			AllowsTransparency = true;
			Closed += AppWindow_Closed;
		}

		#region Event handlers

		void AppWindow_Closed(object sender, EventArgs e)
		{
			if (_hwndSource != null)
			{
				_hwndSource.Dispose();
				_hwndSource = null;
			}
		}

		void CustomizeAltSpace(object sender, KeyEventArgs e)
		{
			var isAltPressed = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
			if (e.Key == Key.System && Keyboard.IsKeyDown(Key.Space) && isAltPressed)
			{
				e.Handled = true;
				CenterOnPrimaryScreen();
			}
		}

		void CenterOnPrimaryScreen()
		{
			Left = (SystemParameters.WorkArea.Width/2) - (Width/2);
			Top = (SystemParameters.WorkArea.Height/2) - (Height/2);
			WindowState = WindowState.Normal;
		}


		void Close_Click(object sender, RoutedEventArgs e)
		{
			Close();
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

		#endregion

		#region interop

		void MoveWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
				return;

			var mPoint = Mouse.GetPosition(this);

			var wpfPoint = PointToScreen(mPoint);
			var x = Convert.ToInt16(wpfPoint.X);
			var y = Convert.ToInt16(wpfPoint.Y);
			var lParam = (int) (uint) x | (y << 16);

			const int wmNclbuttondown = 0x00A1;
			const int htCaption = 0x2;
			SendMessage(_hwndSource.Handle, wmNclbuttondown, htCaption, lParam);
		}

		void ResizeWindow(ResizeDirection direction)
		{
			SendMessage(_hwndSource.Handle, 0x112, (IntPtr) (61440 + direction), IntPtr.Zero);
		}

		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

		#endregion
	}
}