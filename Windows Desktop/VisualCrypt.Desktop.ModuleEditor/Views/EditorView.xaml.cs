using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Cryptography.Portable.Apps.ViewModels;
using VisualCrypt.Desktop.Shared.PrismSupport;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
	[Export(typeof (IEditorView))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class EditorView : IEditorView
	{
		#region Initialization

		public EditorView()
		{
			InitializeComponent();
			Loaded += Editor_Loaded;
		}

		[Import]
		EditorViewModel ViewModel
		{
			set { DataContext = value; }
			get { return DataContext as EditorViewModel; }
		}

		public IEditorContext Context { private get; set; }
		void Editor_Loaded(object sender, RoutedEventArgs args)
		{
			ViewModel.OnViewLoaded(this,Context);

			Application.Current.MainWindow.PreviewKeyDown += MainWindow_PreviewKeyDown;
			
			_textBox1.PreviewMouseWheel += TextBox1_PreviewMouseWheel;
			_textBox1.SizeChanged += _textBox1_SizeChanged;
		
			_textBox1.Focus();
			
			TabControl.Initialized += (s, e) => TabControl.SelectedIndex = 0;

			// Hack to preserve text selection visibility when the window is deactivated.
			// Selection opacity is handled with style trigger on IsFocused.
			Application.Current.MainWindow.Activated += (s, e) => _textBox1.Focus();
			// Zero-size dummy button used to unfocus the TextBox.
			Application.Current.MainWindow.Deactivated += (s, e) => _focusSink.Focus();
			// Stop TextBox from reacting to LostFocus.
			_textBox1.LostFocus += (s, e) => e.Handled = true;
			// end hack.
		}

		

		#endregion

		#region Events

		void _textBox1_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ViewModel.UpdateStatusBar();
		}

		void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// see also: Shell.xaml, Shell.xaml.cs

			// Menu File
			// Print
			if ((e.Key == Key.P && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			     && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
			     && ViewModel.CanExecutePrint()))
				ViewModel.ExecutePrint();

			// Menu Edit
			// Find
			if ((e.Key == Key.F && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			    && ViewModel.CanExecuteFindMenuCommand())
				ViewModel.ExecuteFindMenuCommand();
			//Find Next
			if ((e.Key == Key.F3)
			    && ViewModel.CanExecuteFindNextMenuCommand())
				ViewModel.ExecuteFindNextMenuCommand();
			// Find Previous
			if ((e.Key == Key.F3 && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
			    && ViewModel.CanExecuteFindPreviousMenuCommand())
				ViewModel.ExecuteFindPreviousMenuCommand();
			// Replace
			if ((e.Key == Key.H && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			    && ViewModel.CanExecuteReplaceMenuCommand())
				ViewModel.ExecuteReplaceMenuCommand();
			// Delete Line
			if ((e.Key == Key.Delete && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
			    && ViewModel.CanExecuteDeleteLine())
				ViewModel.ExecuteDeleteLine();
			// GoTo
			if ((e.Key == Key.G && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			    && ViewModel.CanExecuteGoMenuCommand())
				ViewModel.ExecuteGoMenuCommand();
			// Insert Date, Time
			if ((e.Key == Key.F5)
			    && ViewModel.CanExecuteInsertDateTime())
				ViewModel.ExecuteInsertDateTime();

			// Menu Format
			//Font
			if ((e.Key == Key.F && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			     && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
			     && ViewModel.CanExecuteFont()))
				ViewModel.ExecuteFont();

			// Menu View
			// ZoomIn
			if ((e.Key == Key.OemPlus && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			    && ViewModel.CanExecuteZoomIn())
				ViewModel.ExecuteZoomIn();
			// ZoomOut
			if ((e.Key == Key.OemMinus && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			    && ViewModel.CanExecuteZoomOut())
				ViewModel.ExecuteZoomOut();
			// Zoom 100%
			if ((e.Key == Key.D0 && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			    && ViewModel.CanExecuteZoom100())
				ViewModel.ExecuteZoom100();
		}

		void TextBox1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				if (e.Delta > 0)
				{
					if (ViewModel.CanExecuteZoomIn())
						ViewModel.ExecuteZoomIn();
				}
				else
				{
					if (ViewModel.CanExecuteZoomOut())
						ViewModel.ExecuteZoomOut();
				}
			}
		}

		void ToolArea_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateLayout();
		}

		void ToolArea_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == IsVisibleProperty)
			{
				if ((bool)e.NewValue == false)
					_textBox1.Focus();
			}
		}

		#endregion

		#region RoutedCommands

		void CanExecuteFind(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteFindMenuCommand();
		}

		void ExecuteFind(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteFindMenuCommand();
		}

		void CanExecuteFindNext(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteFindNextMenuCommand();
		}

		void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteFindNextMenuCommand();
		}

		void CanExecuteFindPrevious(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteFindPreviousMenuCommand();
		}

		void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteFindPreviousMenuCommand();
		}

		void CanExecuteReplace(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteReplaceMenuCommand();
		}

		void ExecuteReplace(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteReplaceMenuCommand();
		}

		void CanExecuteDeleteLine(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteDeleteLine();
		}

		void ExecuteDeleteLine(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteDeleteLine();
		}

		void CanExecuteGoTo(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteGoMenuCommand();
		}

		void ExecuteGoTo(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteGoMenuCommand();
		}

		void CanExecuteInsertDate(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteInsertDateTime();
		}

		void ExecuteInsertDate(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteInsertDateTime();
		}

		void CanExecuteFont(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteFont();
		}

		void ExecuteFont(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteFont();
		}

		void CanExecuteZoomIn(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteZoomIn();
		}

		void ExecuteZoomIn(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteZoomIn();
		}

		void CanExecuteZoomOut(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteZoomOut();
		}

		void ExecuteZoomOut(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteZoomOut();
		}

		void CanExecuteZoom100(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecuteZoom100();
		}

		void ExecuteZoom100(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecuteZoom100();
		}

		void CanExecutePrint(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.CanExecutePrint();
		}

		void ExecutePrint(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.ExecutePrint();
		}

		#endregion

		#region IEditorView

		public TextBox TextBox1
		{
			get { return _textBox1; }
		}

		public TextBox TextBoxFind
		{
			get { return _textBoxFind; }
		}

		public TextBox TextBoxFindReplace
		{
			get { return _textBoxFindReplace; }
		}

		public TextBox TextBoxGoTo
		{
			get { return _textBoxGoTo; }
		}

		public UIElement EditorControl
		{
			get { return this; }
		}

		#endregion
	}
}