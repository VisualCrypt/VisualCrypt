using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Desktop.Services;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Desktop.Views
{
    public partial class MainWindow
    {
        readonly PortableMainViewModel _viewModel;
        readonly SettingsManager _settingsManager;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();

            DataContext = _viewModel;

            PreviewKeyDown += ShellWindow_PreviewKeyDown;
            Closing += MainWindow_Closing;

            AllowDrop = true;
            PreviewDragEnter += MainWindow_PreviewDragEnter;
            PreviewDragLeave += MainWindow_PreviewDragLeave;
            PreviewDrop += MainWindow_PreviewDrop;
        }

    

        async void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            await _viewModel.ExitCommand.Execute(e);
        }

        async void ShellWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // This was in TextBox1_PreviewKeyDown, does this still work?
            if ((e.Key == Key.R && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                && _viewModel.ClearPasswordCommand.CanExecute())
                await _viewModel.ClearPasswordCommand.Execute();

            if (e.Key == Key.F12)
                _settingsManager.EditorSettings.IsStatusBarVisible = !_settingsManager.EditorSettings.IsStatusBarVisible;
            if (e.Key == Key.Escape)
                _settingsManager.EditorSettings.IsToolAreaVisible = !_settingsManager.EditorSettings.IsToolAreaVisible;

            if (e.Key == Key.W && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                _settingsManager.EditorSettings.IsWordWrapChecked = !_settingsManager.EditorSettings.IsWordWrapChecked;
            if (e.Key == Key.L && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                _settingsManager.EditorSettings.IsSpellCheckingChecked =
                    !_settingsManager.EditorSettings.IsSpellCheckingChecked;
        }


        void MainWindow_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                ContentEditorRegion.IsHitTestVisible = false;
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
                    _viewModel.OpenFileFromDragDrop(files[0]);
                }
            }
            finally
            {
                ContentEditorRegion.IsHitTestVisible = true;
            }
        }

        void MainWindow_PreviewDragLeave(object sender, DragEventArgs e)
        {
            ContentEditorRegion.IsHitTestVisible = true;
        }



    }
}