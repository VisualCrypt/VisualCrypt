using Windows.UI.Xaml.Controls;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using Windows.UI.Xaml;
using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI;
using VisualCrypt.Windows.Styles;
using Windows.UI.Xaml.Navigation;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class FilesPage
    {
        readonly FilesPageViewModel _viewModel;
        FileReference _selectedFileReference;
        static FilesPage _pageReference;

        public FilesPage()
        {
            InitializeComponent();
            _viewModel = Service.Get<FilesPageViewModel>();
            Loaded +=  (s, e) =>
            {
                _viewModel.PropertyChanged += _viewModel_PropertyChanged;
                
            };

            FilesListView.SelectionChanged += FilesListView_SelectionChanged;
            _pageReference = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // workaround on Window sizing bug with multiple screens
            var content = Window.Current.Content;
            var grid = new Grid();
            grid.Loaded += delegate { Window.Current.Content = content; };
            Window.Current.Content = grid;

            await _viewModel.OnNavigatedToCompleteAndLoaded();
        }

        void DisplayDialogCommon()
        {
            TopAppBar.Visibility = Visibility.Collapsed;

            FilesListView.IsEnabled = false;
            FilesListView.Opacity = 0.8;

            BottomAppBar.Visibility = Visibility.Collapsed;
            BackgroundGrid.Background = MoreColors.BackgroundGridDisabledColorBrush;
        }

        void HideDialogCommon()
        {
            TopAppBar.Visibility = Visibility.Visible;
            FilesListView.IsEnabled = true;
            FilesListView.Opacity = 1;
            FilesListView.Focus(FocusState.Programmatic);

            BottomAppBar.Visibility = Visibility.Visible;
            BackgroundGrid.Background = MoreColors.WhiteBrush;
        }


        internal void DisplayFilenameDialog()
        {
            DisplayDialogCommon();
            FilenameUserControl.Visibility = Visibility.Visible;
        }

        internal void HideFilenameDialog()
        {
            HideDialogCommon();
            FilenameUserControl.Visibility = Visibility.Collapsed;
        }

        private void _viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.IsEditMode))
            {
                if (_viewModel.IsEditMode)
                {
                    FilesListView.SelectionMode = ListViewSelectionMode.Multiple;
                    FilesListView.IsItemClickEnabled = false;
                }
                else
                {
                    FilesListView.SelectionMode = ListViewSelectionMode.None;
                    FilesListView.IsItemClickEnabled = true;
                }


            }
        }

        async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var fileReference = e.ClickedItem as FileReference;
            await _viewModel.NavigateToOpenCommand.Execute(fileReference);
        }

        void FilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilesListView.SelectedItems.Count == 1)
            {
                _viewModel.SingleSelectedFileReference = FilesListView.SelectedItem as FileReference;
                //EnableContentTransitions();
            }
            else if (FilesListView.SelectedItems.Count > 1)
            {
                _viewModel.MultiSelectedFileReferences = new List<FileReference>();
                foreach (var item in FilesListView.SelectedItems)
                {
                    _viewModel.MultiSelectedFileReferences.Add((FileReference)item);
                }
            }
            else
            {
            }
            _viewModel.SelectedItemsCount = FilesListView.SelectedItems.Count;
            _viewModel.RenameCommand.RaiseCanExecuteChanged();
            _viewModel.DeleteCommand.RaiseCanExecuteChanged();
        }



        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            if (_selectedFileReference != null)
            {
                // Contacts.Remove(_selectedFileReference);

                if (FilesListView.Items.Count > 0)
                {
                    FilesListView.SelectedIndex = 0;
                    _selectedFileReference = FilesListView.SelectedItem as FileReference;
                }
                else
                {
                    // Details view is collapsed, in case there is not items.
                    // DetailContentPresenter.Visibility = Visibility.Collapsed;
                    _selectedFileReference = null;
                }
            }
        }
        private void DeleteItems(object sender, RoutedEventArgs e)
        {
            if (FilesListView.SelectedIndex != -1)
            {
                List<FileReference> selectedItems = new List<FileReference>();
                foreach (FileReference contact in FilesListView.SelectedItems)
                {
                    selectedItems.Add(contact);
                }
                foreach (FileReference contact in selectedItems)
                {
                    //Contacts.Remove(contact);
                }
                if (FilesListView.Items.Count > 0)
                {
                    FilesListView.SelectedIndex = 0;
                    _selectedFileReference = FilesListView.SelectedItem as FileReference;
                }
                else
                {
                    //DetailContentPresenter.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void SelectItems(object sender, RoutedEventArgs e)
        {
            if (FilesListView.Items.Count > 0)
            {
                //VisualStateManager.GoToState(this, MultipleSelectionState.Name, true);
            }
        }
        private void CancelSelection(object sender, RoutedEventArgs e)
        {
            //if (PageSizeStatesGroup.CurrentState == NarrowState)
            //{
            //    VisualStateManager.GoToState(this, MasterState.Name, true);
            //}
            //else
            //{
            //    VisualStateManager.GoToState(this, MasterDetailsState.Name, true);
            //}
        }

        internal static FilesPage GetPageReference()
        {
            return _pageReference;
        }
    }
}
