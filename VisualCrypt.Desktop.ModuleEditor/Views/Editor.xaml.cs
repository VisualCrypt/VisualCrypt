﻿using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisualCrypt.Desktop.Shared;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
    [Export(typeof(IEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Editor : IEditor
    {
        public Editor()
        {
            InitializeComponent();
            Loaded += Editor_Loaded;
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.OnEditorInitialized(TextBox1);
            TextBox1.TextChanged += TextBox1_TextChanged;
            TextBox1.SelectionChanged += TextBox1_SelectionChanged;
            TextBox1.PreviewKeyDown += TextBox1_PreviewKeyDown;

            TextBox1.SpellCheck.SpellingReform = SpellingReform.Postreform;
            TextBox1.PreviewMouseWheel += TextBox1_PreviewMouseWheel;
            TextBox1.Focus();
        }


        [Import]
        EditorViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as EditorViewModel; }

        }

        void TextBox1_PreviewKeyDown(object sender, KeyEventArgs e)
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
                && ViewModel.CanExecuteFind())
                ViewModel.ExecuteFind();
            //Find Next
            if ((e.Key == Key.F3)
               && ViewModel.CanExecuteFindNext())
                ViewModel.ExecuteFindNext();
            // Find Previous
            if ((e.Key == Key.F3 && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                && ViewModel.CanExecuteFindPrevious())
                ViewModel.ExecuteFindPrevious();
            // Replace
            if ((e.Key == Key.H && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                && ViewModel.CanExecuteReplace())
                ViewModel.ExecuteReplace();
            // Delete Line
            if ((e.Key == Key.Delete && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                && ViewModel.CanExecuteDeleteLine())
                ViewModel.ExecuteDeleteLine();
            // GoTo
            if ((e.Key == Key.G && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
               && ViewModel.CanExecuteGoTo())
                ViewModel.ExecuteGoTo();
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

        void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ExecuteTextChangedCommand(e);
        }

        void TextBox1_SelectionChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.ExecuteSelectionChangedCommand(routedEventArgs);
        }

        #region EditorCommands

        void CanExecuteFind(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = ViewModel.CanExecuteFind();
        }

        void ExecuteFind(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteFind();
        }

        private void CanExecuteFindNext(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteFindNext();
        }

        private void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteFindNext();
        }

        private void CanExecuteFindPrevious(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteFindPrevious();
        }

        private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteFindPrevious();
        }

        private void CanExecuteReplace(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteReplace();
        }

        private void ExecuteReplace(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteReplace();
        }

        private void CanExecuteDeleteLine(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteDeleteLine();
        }

        private void ExecuteDeleteLine(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteDeleteLine();
        }

        private void CanExecuteGoTo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteGoTo();
        }

        private void ExecuteGoTo(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteGoTo();
        }

        private void CanExecuteInsertDate(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteInsertDateTime();
        }

        private void ExecuteInsertDate(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteInsertDateTime();
        }

        private void CanExecuteFont(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteFont();
        }

        private void ExecuteFont(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteFont();
        }

        private void CanExecuteZoomIn(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteZoomIn();
        }

        private void ExecuteZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteZoomIn();
        }

        private void CanExecuteZoomOut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteZoomOut();
        }

        private void ExecuteZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteZoomOut();
        }

        private void CanExecuteZoom100(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecuteZoom100();
        }

        private void ExecuteZoom100(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteZoom100();
        }

        private void CanExecutePrint(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.CanExecutePrint();
        }

        private void ExecutePrint(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecutePrint();
        }

        #endregion
    }
}
