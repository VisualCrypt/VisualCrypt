using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.PrismSupport;

namespace VisualCrypt.Desktop.Views
{
    [Export]
    public partial class ShellWindow
    {
        readonly IRegionManager _regionManager;

        [Import]
        ShellViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as ShellViewModel; }
        }

        [ImportingConstructor]
        public ShellWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;

            Loaded += ShellWindow_Loaded;

            PreviewKeyDown += ShellWindow_PreviewKeyDown;
            Closing += MainWindow_Closing;

            AllowDrop = true;
            PreviewDragEnter += MainWindow_PreviewDragEnter;
            PreviewDragLeave += MainWindow_PreviewDragLeave;
            PreviewDrop += MainWindow_PreviewDrop;
        }

        void ShellWindow_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ActivateEditor();
        }


        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.ExitCommand.Execute(e);
        }

        void ShellWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // This was in TextBox1_PreviewKeyDown, does this still work?
            if ((e.Key == Key.R && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                && ViewModel.ClearPasswordCommand.CanExecute())
                ViewModel.ClearPasswordCommand.Execute();

            if (e.Key == Key.F12)
                SettingsManager.Instance.EditorSettings.IsStatusBarChecked = !SettingsManager.Instance.EditorSettings.IsStatusBarChecked;
            if (e.Key == Key.Escape)
                SettingsManager.Instance.EditorSettings.IsToolAreaChecked = !SettingsManager.Instance.EditorSettings.IsToolAreaChecked;

            if (e.Key == Key.W && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                SettingsManager.Instance.EditorSettings.IsWordWrapChecked = !SettingsManager.Instance.EditorSettings.IsWordWrapChecked;
            if (e.Key == Key.L && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                SettingsManager.Instance.EditorSettings.IsSpellCheckingChecked =
                    !SettingsManager.Instance.EditorSettings.IsSpellCheckingChecked;
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
                    var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                    ViewModel.OpenFileFromDragDrop(files[0]);
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


        void ActivateEditor()
        {
            var mainRegion = _regionManager.Regions[RegionNames.EditorRegion];
            if (mainRegion == null)
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture,
                        "The region {0} is missing and has probably not been defined in Xaml.",
                        RegionNames.EditorRegion));

            var view = mainRegion.GetView(typeof (IEditorView).Name) as IEditorView;
            if (view == null)
            {
                view = ServiceLocator.Current.GetInstance<IEditorView>();
                mainRegion.Add(view, typeof (IEditorView).Name); // automatically activates the view
            }
            else
            {
                mainRegion.Activate(view);
            }
        }
    }
}