using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Prism.Commands;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Language.Strings;
using VisualCrypt.UWP.Services;

namespace VisualCrypt.UWP.Pages
{
    public sealed partial class SettingsPage : INotifyPropertyChanged
    {
        readonly IMessageBoxService _messageBoxService;
        readonly SettingsManager _settingsManager;
        readonly ResourceWrapper _resourceWrapper;
        readonly string _title;
        byte _initialLogRounds;

        public SettingsPage()
        {
            InitializeComponent();
            _messageBoxService = Service.Get<IMessageBoxService>();
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _title = _resourceWrapper.miVCSettings.NoDots();

            LogRounds = _settingsManager.CryptographySettings.LogRounds;
            _initialLogRounds = _logRounds;

            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            e.Handled = true;
            Frame.Navigate(typeof(FilesPage), new EntranceNavigationTransitionInfo());
        }

        string Title => _title;
        ResourceWrapper ResourceWrapper => _resourceWrapper;

        readonly byte _defaultBCryptRoundsLog2 = 10;

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
            LogRounds = _defaultBCryptRoundsLog2;
        }

        bool CanExecuteDefaultsCommand()
        {
            return Math.Abs(LogRounds - _defaultBCryptRoundsLog2) > 0.5;
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
            if (logRounds == _defaultBCryptRoundsLog2)
            {
                TextBlockWarning.Text = _resourceWrapper.sett_warn_neutral;
                return;
            }
            TextBlockWarning.Text = logRounds > _defaultBCryptRoundsLog2
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
