using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace VisualCrypt.Desktop.Shared.Controls
{
    public class AppWindow : Window
    {
        static AppWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AppWindow),
                new FrameworkPropertyMetadata(typeof(AppWindow)));
        }

        Button _restoreButton;
        bool _restoreIfMove;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var minimizeButton = (Button)GetTemplateChild("minimizeButton");
            if (minimizeButton != null)
                minimizeButton.Click += Minimize_Click;

            _restoreButton = (Button)GetTemplateChild("restoreButton");
            if (_restoreButton != null)
                _restoreButton.Click += Restore_Click;


            var closeButton = (Button)GetTemplateChild("closeButton");
            if (closeButton != null)
                closeButton.Click += Close_Click;

            var moveRectangle = (Rectangle)GetTemplateChild("moveRectangle");

            if (moveRectangle != null)
            {
                moveRectangle.PreviewMouseLeftButtonDown += MoveRectangle_PreviewMouseLeftButtonDown;
                moveRectangle.PreviewMouseLeftButtonUp += MoveRectangle_PreviewMouseLeftButtonUp;
                moveRectangle.PreviewMouseMove += MoveRectangle_PreviewMouseMove;
            }

            var resizeGrid = (Panel)GetTemplateChild("resizeGrid");
            if (resizeGrid != null)
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
            SwitchWindowState();
        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void Maximize()
        {
            WindowState = WindowState.Maximized;
            var test = this;
            _mLeft = SystemParameters.VirtualScreenLeft;
            _mTop = SystemParameters.VirtualScreenTop;
            Console.WriteLine("Saving _mLeft = {0}, _mTop = {1}", _mLeft, _mTop);
        }

        void MoveRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (e.ClickCount == 2)
            {
                if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip))
                {
                    SwitchWindowState();
                }
                return;
            }

            if (WindowState == WindowState.Maximized)
            {
                _restoreIfMove = true;
                return;
            }
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MoveRectangle_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _restoreIfMove = false;
            Console.WriteLine("After Realease");
            Console.WriteLine(Left);
            Console.WriteLine(Top);
        }

        private void MoveRectangle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_restoreIfMove)
            {
                _restoreIfMove = false;

                SwitchWindowState();

                POINT lMousePosition;
                GetCursorPos(out lMousePosition);
                if (!(lMousePosition.X > SystemParameters.WorkArea.Width))
                {
                    Left = 0;
                    Top = 0;
                    DragMove();
                }
            }
        }

        void ResizeRectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                Cursor = Cursors.Arrow;
        }

        void ResizeRectangle_MouseMove(Object sender, MouseEventArgs e)
        {
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

        #region interop

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        HwndSource _hwndSource;

        void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        protected override void OnInitialized(EventArgs e)
        {
            SourceInitialized += OnSourceInitialized;
          
            base.OnInitialized(e);
        }

        void OnSourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            if (_hwndSource != null) 
                _hwndSource.AddHook(WindowProc);
        }

        #endregion



        static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }


        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
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

            var lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

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

        double _mLeft;
        double _mTop;
        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        _restoreButton.Content = 2;
                        Maximize();
                        break;
                    }
                case WindowState.Maximized:
                    {
                      
                       
                        _restoreButton.Content = 1;
                        WindowState = WindowState.Normal;
                        break;
                    }
            }
        }









        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
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
        // ReSharper disable InconsistentNaming
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
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
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
}