using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VisualCrypt.Desktop.Views
{
    public partial class MainWindow
    {
        readonly MainWindowViewModel _mainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _mainWindowViewModel = new MainWindowViewModel(this);
            DataContext = _mainWindowViewModel;

            Closing += MainWindow_Closing;
            SizeChanged += MainWindow_SizeChanged;

            AllowDrop = true;
            PreviewDragEnter += MainWindow_PreviewDragEnter;
            PreviewDragLeave += MainWindow_PreviewDragLeave;
            PreviewDrop += MainWindow_PreviewDrop;

            // Hack to preserve text selection visibility when the window is deactivated.
            // Selection opacity is handled with style triggers.
            Activated += (s, e) => TextBox1.Focus();
            Deactivated += (s, e) => Button1.Focus();  // zero size dummy button
            TextBox1.LostFocus += (s, e) => e.Handled = true;
            // end hack.

            TextBox1.TextChanged += TextBox1_TextChanged;
            TextBox1.SelectionChanged += TextBox1_SelectionChanged;
            TextBox1.PreviewKeyDown += TextBox1_PreviewKeyDown;

            TextBox1.PreviewMouseWheel += TextBox1_PreviewMouseWheel;

            _mainWindowViewModel.OnMainWindowInitialized();
            TextBox1.Focus();
        }

   

        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _mainWindowViewModel.UpdateStatusBar();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            _mainWindowViewModel.ExitCommand.Execute(e);
        }

        void MainWindow_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                TextBox1.IsHitTestVisible = false;
                e.Effects = DragDropEffects.Copy;
            }
        }

        void MainWindow_PreviewDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    _mainWindowViewModel.OpenFileFromDragDrop(files[0]);
                }
            }
            finally
            {
                TextBox1.IsHitTestVisible = true;
            }

        }

        void MainWindow_PreviewDragLeave(object sender, DragEventArgs e)
        {
            TextBox1.IsHitTestVisible = true;
        }

        void TextBox1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Delete && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                && _mainWindowViewModel.DeleteLineCommand.CanExecute())
                _mainWindowViewModel.DeleteLineCommand.Execute();

            if ((e.Key == Key.OemPlus && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                && _mainWindowViewModel.ZoomInCommand.CanExecute())
                _mainWindowViewModel.ZoomInCommand.Execute();

            if ((e.Key == Key.R && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                && _mainWindowViewModel.ClearPasswordCommand.CanExecute())
                _mainWindowViewModel.ClearPasswordCommand.Execute();
        }

        void TextBox1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta > 0)
                {
                    if (_mainWindowViewModel.ZoomInCommand.CanExecute())
                        _mainWindowViewModel.ZoomInCommand.Execute();
                }
                else
                {
                    if (_mainWindowViewModel.ZoomOutCommand.CanExecute())
                        _mainWindowViewModel.ZoomOutCommand.Execute();
                }
            }
        }

        void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            _mainWindowViewModel.TextChangedCommand.Execute(e);
        }

        void TextBox1_SelectionChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            _mainWindowViewModel.SelectionChangedCommand.Execute(routedEventArgs);
        }



        void Hyperlink_SetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindowViewModel.ShowSetPasswordDialogCommand.CanExecute())
                _mainWindowViewModel.ShowSetPasswordDialogCommand.Execute();
        }

        void TextBlock_ClearPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(_mainWindowViewModel.ClearPasswordCommand.CanExecute())
                _mainWindowViewModel.ClearPasswordCommand.Execute();
        }
    }
}