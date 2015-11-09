using Windows.UI.Xaml.Controls;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using Windows.UI.Xaml;
using System.Collections.Generic;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class FilesPage
    {
        readonly FilesPageViewModel _viewModel;
        FileReference _selectedFileReference;

        public FilesPage()
        {
            InitializeComponent();
            _viewModel = Service.Get<FilesPageViewModel>();
            Loaded += async (s, e) =>
            {
                _viewModel.PropertyChanged += _viewModel_PropertyChanged;
                await _viewModel.OnNavigatedToCompleteAndLoaded();
            };

            FilesListView.SelectionChanged += FilesListView_SelectionChanged;

        }
            
        
        private void _viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.IsEditMode))
            {
                if(_viewModel.IsEditMode)
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
                foreach(var item in FilesListView.SelectedItems)
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
    }
}
