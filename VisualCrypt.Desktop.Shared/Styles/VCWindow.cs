#region usings
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Panel = System.Windows.Controls.Panel;
#endregion

namespace VisualCrypt.Desktop.Shared.Styles
{
	/// <summary>
	/// The FundRep Window Control that allows us to meet all 
	/// visual expectations of our customer.
	/// </summary>
	public class VCWindow : Window
	{
		static VCWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(VCWindow),
				new FrameworkPropertyMetadata(typeof(VCWindow)));
		}

		Button m_RestoreButton;

		/// <summary>
		/// Wire up the Template.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			((Button)GetTemplateChild("minimizeButton")).Click += Minimize_Click;
			m_RestoreButton = (Button)GetTemplateChild("restoreButton");
			m_RestoreButton.Click += Restore_Click;
			((Button)GetTemplateChild("closeButton")).Click += Close_Click;

			((Rectangle)GetTemplateChild("moveRectangle")).PreviewMouseLeftButtonDown += MoveRectangle_PreviewMouseLeftButtonDown;

			var resizeGrid = (Panel)GetTemplateChild("resizeGrid");
			foreach (UIElement rectangle in resizeGrid.Children)
			{
				rectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
				rectangle.MouseMove += ResizeRectangle_MouseMove;
				rectangle.MouseLeave += ResizeRectangle_MouseLeave;
			}
		}


		#region Click events




		void Minimize_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		void Restore_Click(object sender, RoutedEventArgs e)
		{
			switch (WindowState)
			{
				case WindowState.Normal:
					MaxHeight = Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea.Height;
					MaxWidth = Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea.Width;
					WindowState = WindowState.Maximized;
					m_RestoreButton.Content = 2;
					break;
				case WindowState.Maximized:
					WindowState = WindowState.Normal;
					m_RestoreButton.Content = 1;
					break;
			}
		}

		void Close_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		void MoveRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
				Restore_Click(sender, e);

			if (Mouse.LeftButton == MouseButtonState.Pressed)
				DragMove();
		}

		void ResizeRectangle_MouseLeave(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
				Cursor = Cursors.Arrow;
		}

		void ResizeRectangle_MouseMove(Object sender, MouseEventArgs e)
		{
			var rectangle = sender as Rectangle;
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
			var rectangle = sender as Rectangle;
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

		#region static







		#endregion

		#region interop

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

		HwndSource m_HwndSource;

		void ResizeWindow(ResizeDirection direction)
		{
			SendMessage(m_HwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
		}

		protected override void OnInitialized(EventArgs e)
		{
			SourceInitialized += OnSourceInitialized;
			base.OnInitialized(e);
		}

		void OnSourceInitialized(object sender, EventArgs e)
		{
			m_HwndSource = (HwndSource)PresentationSource.FromVisual(this);
		}

		#endregion
	}
}