﻿using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using VisualCrypt.Desktop.Shared.App;

namespace VisualCrypt.Desktop.Views
{
    [Export]
    public partial class Shell
    {
        /// <summary>
        /// Sets the ViewModel.
        /// </summary>
        /// <remarks>
        /// This set-only property is annotated with the <see cref="ImportAttribute"/> so it is injected by MEF with
        /// the appropriate view model.
        /// </remarks>
        [Import]
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Needs to be a property to be composed by MEF")]
        ShellViewModel ViewModel
        {
            set
            {
                DataContext = value;
            }
            get { return DataContext as ShellViewModel; }
        }

        public Shell()
        {
            InitializeComponent();

            this.Loaded += Shell_Loaded;
            this.PreviewKeyDown += ShellWindow_PreviewKeyDown;
            //Closing += MainWindow_Closing;
            //SizeChanged += MainWindow_SizeChanged;

            //AllowDrop = true;
            //PreviewDragEnter += MainWindow_PreviewDragEnter;
            //PreviewDragLeave += MainWindow_PreviewDragLeave;
            //PreviewDrop += MainWindow_PreviewDrop;

            // Hack to preserve text selection visibility when the window is deactivated.
            // Selection opacity is handled with style triggers.
            //Activated += (s, e) => TextBox1.Focus();
            Deactivated += (s, e) => Button1.Focus();  // zero size dummy button
            //TextBox1.LostFocus += (s, e) => e.Handled = true;
            // end hack.



            // _shellViewModel.OnMainWindowInitialized();

        }

        void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }

        void ShellWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // This was in TextBox1_PreviewKeyDown, does this still work?
            if ((e.Key == Key.R && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                && ViewModel.CanExecuteClearPasswordCommand())
                ViewModel.ExecuteClearPasswordCommand();

            if (e.Key == Key.F12)
                SettingsManager.EditorSettings.IsStatusBarChecked = !SettingsManager.EditorSettings.IsStatusBarChecked;

            if(e.Key == Key.W && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                SettingsManager.EditorSettings.IsWordWrapChecked = !SettingsManager.EditorSettings.IsWordWrapChecked;
            if (e.Key == Key.L && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                SettingsManager.EditorSettings.IsSpellCheckingChecked = !SettingsManager.EditorSettings.IsSpellCheckingChecked;
        }

        //void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    _shellViewModel.UpdateStatusBar();
        //}

        //void MainWindow_Closing(object sender, CancelEventArgs e)
        //{
        //    _shellViewModel.ExitCommand.Execute(e);
        //}

        //void MainWindow_PreviewDragEnter(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        TextBox1.IsHitTestVisible = false;
        //        e.Effects = DragDropEffects.Copy;
        //    }
        //}

        //void MainWindow_PreviewDrop(object sender, DragEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //        {
        //            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
        //            _shellViewModel.OpenFileFromDragDrop(files[0]);
        //        }
        //    }
        //    finally
        //    {
        //        TextBox1.IsHitTestVisible = true;
        //    }

        //}

        //void MainWindow_PreviewDragLeave(object sender, DragEventArgs e)
        //{
        //    TextBox1.IsHitTestVisible = true;
        //}

        //void TextBox1_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if ((e.Key == Key.Delete && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
        //        && _shellViewModel.DeleteLineCommand.CanExecute())
        //        _shellViewModel.DeleteLineCommand.Execute();

        //    if ((e.Key == Key.OemPlus && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
        //        && _shellViewModel.ZoomInCommand.CanExecute())
        //        _shellViewModel.ZoomInCommand.Execute();

        //    if ((e.Key == Key.R && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
        //        && _shellViewModel.ClearPasswordCommand.CanExecute())
        //        _shellViewModel.ClearPasswordCommand.Execute();
        //}

        //void TextBox1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
        //    {
        //        if (e.Delta > 0)
        //        {
        //            if (_shellViewModel.ZoomInCommand.CanExecute())
        //                _shellViewModel.ZoomInCommand.Execute();
        //        }
        //        else
        //        {
        //            if (_shellViewModel.ZoomOutCommand.CanExecute())
        //                _shellViewModel.ZoomOutCommand.Execute();
        //        }
        //    }
        //}

        //void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    _shellViewModel.TextChangedCommand.Execute(e);
        //}

        //void TextBox1_SelectionChanged(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    _shellViewModel.SelectionChangedCommand.Execute(routedEventArgs);
        //}



        //void Hyperlink_SetPassword_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_shellViewModel.ShowSetPasswordDialogCommand.CanExecute())
        //        _shellViewModel.ShowSetPasswordDialogCommand.Execute();
        //}

        //void TextBlock_ClearPassword_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if(_shellViewModel.ClearPasswordCommand.CanExecute())
        //        _shellViewModel.ClearPasswordCommand.Execute();
        //}
        private void Hyperlink_SetPassword_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void TextBlock_ClearPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}