using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Prism.Commands;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Language.Strings;
using VisualCrypt.UWP.Services;

namespace VisualCrypt.UWP.Pages
{
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        readonly IMessageBoxService _messageBoxService;
        readonly IEncryptionService _encryptionService;
        readonly SettingsManager _settingsManager;
        readonly ResourceWrapper _resourceWrapper;
        readonly string _title;
        byte _initialLogRounds;

        public SettingsPage()
        {
            InitializeComponent();
            _messageBoxService = Service.Get<IMessageBoxService>();
            _encryptionService = Service.Get<IEncryptionService>();
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _title = _resourceWrapper.miVCSettings.NoDots();

            LogRounds = _settingsManager.CryptographySettings.LogRounds;
            _initialLogRounds = _logRounds;
        }

        string Title => _title;
        ResourceWrapper ResourceWrapper => _resourceWrapper;

        byte DefaultBCryptRoundsLog2 = BCrypt.DefaultBCryptRoundsLog2;

        void AppBarButton_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FilesPage));
        }

        public DelegateCommand SaveCommand
        {
            get
            {
                if (_saveCommand != null)
                    return _saveCommand;
                _saveCommand = new DelegateCommand(ExecuteSaveCommand, () => _logRounds != _initialLogRounds);
                return _saveCommand;
            }
        }

        DelegateCommand _saveCommand;

        void ExecuteSaveCommand()
        {
            try
            {
                _settingsManager.CryptographySettings.LogRounds = _logRounds;
                _initialLogRounds = _logRounds;
                SaveCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }


        DelegateCommand _defaultsCommand;

        public DelegateCommand DefaultsCommand
        {
            get
            {
                if (_defaultsCommand != null)
                    return _defaultsCommand;
                _defaultsCommand = new DelegateCommand(ExecuteDefaultsCommand, CanExecuteDefaultsCommand);
                return _defaultsCommand;
            }
        }

        void ExecuteDefaultsCommand()
        {
            LogRounds = BCrypt.DefaultBCryptRoundsLog2;
        }

        bool CanExecuteDefaultsCommand()
        {
            return LogRounds != BCrypt.DefaultBCryptRoundsLog2;
        }

      


       


        public double LogRounds
        {
            get { return _logRounds; }
            set
            {
                _logRounds = Convert.ToByte(value);
                OnPropertyChanged();
                SetWarningText(_logRounds);
                DefaultsCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        byte _logRounds;

        void SetWarningText(byte logRounds)
        {
            if (logRounds == BCrypt.DefaultBCryptRoundsLog2)
            {
                TextBlockWarning.Text = _resourceWrapper.sett_warn_neutral;
                return;
            }
            TextBlockWarning.Text = logRounds > BCrypt.DefaultBCryptRoundsLog2
                ? _resourceWrapper.sett_warn_high
                : _resourceWrapper.sett_warn_low;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
