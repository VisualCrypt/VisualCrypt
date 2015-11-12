using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Applications.ViewModels
{
    public class PortableFilenameDialogViewModel : ViewModelBase
    {
        readonly IMessageBoxService _messageBoxService;
        readonly ResourceWrapper _resourceWrapper;


        Action<Tuple<bool, string>> _setResult;
        string[] _filenames;
        FileDialogMode _fileDialogMode;

        public ResourceWrapper ResourceWrapper { get { return _resourceWrapper; } }

        public void Init(FileDialogMode fileDialogMode, string[] filenames, Action<Tuple<bool, string>> setResult)
        {
            _fileDialogMode = fileDialogMode;
            _filenames = filenames;
            _setResult = setResult;
            SetMode(fileDialogMode);
        }

        public PortableFilenameDialogViewModel()
        {
            _messageBoxService = Service.Get<IMessageBoxService>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
        }


        #region Bound Properties


        public string Filename
        {
            get { return _filename; }
            set
            {
                if (_filename != value && value != null)
                {
                    _filename = value;

                    if (_filename.ToLowerInvariant().Contains(".visualcrypt"))
                        _filename = _filename.Remove(value.IndexOf(".visualcrypt"));

                    OnPropertyChanged();
                    OKCommand.RaiseCanExecuteChanged();
                }
            }

        }
        string _filename = string.Empty;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        string _title = string.Empty;

        public string OKButtonContent
        {
            get { return _okButtonContent; }
            set
            {
                _okButtonContent = value;
                OnPropertyChanged();
            }
        }
        string _okButtonContent = string.Empty;



        #endregion



        #region private Methods

        void SetMode(FileDialogMode filenameDialogMode)
        {
            switch (filenameDialogMode)
            {
                case FileDialogMode.SaveAs:
                    Title = _resourceWrapper.fileDlgSetName;
                    OKButtonContent = _resourceWrapper.termOK;
                    break;
                case FileDialogMode.Rename:
                    Title = _resourceWrapper.fileDlgRename;
                    OKButtonContent = _resourceWrapper.termOK;
                    Filename = _filenames[0];
                    break;
                case FileDialogMode.Delete:
                    Title = _resourceWrapper.fileDlgDelete;
                    OKButtonContent = _resourceWrapper.termOK;
                    break;
                case FileDialogMode.DeleteMany:
                    Title = _resourceWrapper.fileDlgDeleteMany;
                    OKButtonContent = _resourceWrapper.termOK;
                    break;
            }
        }
        bool IsFilenameValid(string newFilename)
        {
            if (string.IsNullOrWhiteSpace(newFilename))
                return false;

            var disallowedChars = new List<char>();
            disallowedChars.AddRange(Path.GetInvalidFileNameChars());
            disallowedChars.AddRange(Path.GetInvalidPathChars());
            disallowedChars.AddRange(new[] { '*', '?', ':', '/', '\\' });
            foreach (var c in newFilename)
            {
                if (disallowedChars.Contains(c))
                    return false;
            }

            return true;

        }
        public void Close(bool setClicked)
        {

        }

        #endregion

        #region SetPasswordCommand

        public DelegateCommand OKCommand
            => CreateCommand(ref _okCommand, ExecuteOKCommand, CanExecuteOKCommand);
        DelegateCommand _okCommand;

        async void ExecuteOKCommand()
        {
            try
            {
                _setResult(new Tuple<bool, string>(true, _filename.Trim()));
            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError(e);
            }
            finally
            {
                _filename = string.Empty;
            }
        }

        bool CanExecuteOKCommand()
        {
            switch (_fileDialogMode)
            {
                case FileDialogMode.SaveAs:
                    return IsFilenameValid(_filename);
                case FileDialogMode.Rename:
                    return IsFilenameValid(_filename) && _filename != _filenames[0];
            }
            throw new NotImplementedException(_fileDialogMode.ToString());
        }

        #endregion



        #region CancelCommand

        public DelegateCommand CancelCommand
            => CreateCommand(ref _cancelCommand, ExecuteCancelCommand, () => true);
        DelegateCommand _cancelCommand;

        void ExecuteCancelCommand()
        {
            Filename = string.Empty;
            _setResult(new Tuple<bool, string>(false, null));
        }

        #endregion

    }
}
