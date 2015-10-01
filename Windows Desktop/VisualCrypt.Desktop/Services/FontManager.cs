﻿using System;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Desktop.Settings;
using VisualCrypt.Desktop.Views.Fonts;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Desktop.Services
{
    class FontManager : IFontManager
    {
        readonly IWindowManager _windowManager;
        readonly SettingsManager _settingsManager;
        readonly TextBoxController _textBoxController;
        readonly IMessageBoxService _messageBoxService;
        readonly IParamsProvider _paramsProvider;
        readonly ResourceWrapper _resourceWrapper;

        public FontManager()
        {
            _windowManager = Service.Get<IWindowManager>();
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _textBoxController = Service.Get<ITextBoxController>(TextBoxName.TextBox1) as TextBoxController;
            _messageBoxService = Service.Get<IMessageBoxService>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _paramsProvider = Service.Get<IParamsProvider>();

            _resourceWrapper.Info.CultureChanged += delegate
            {
                UpdateZoomLevelMenuText();
            };
        }

        public void ApplyFontsFromSettingsToEditor()
        {
            var fontSettings = (FontSettings)_settingsManager.FontSettings;
            _textBoxController.ApplyFontSettings(fontSettings);
        }

        public async void ShowFontChooserAndApplyChoiceToEditor()
        {
           
            _paramsProvider.SetParams(typeof(Font), _textBoxController.Text);
            var fontDialog = await _windowManager.GetDialogFromShowDialogAsyncWhenClosed<Font>();


            if (fontDialog.DialogResult == true)
            {
                ExecuteZoom100();
            }
        }

        void UpdateZoomLevelMenuText()
        {

            var fontSettings = (FontSettings)_settingsManager.FontSettings;
            var zoomLevel = (int)((_textBoxController.FontSize / fontSettings.FontSize) * 100);
            var zoomLevelMenuText = string.Format(_resourceWrapper.miViewZoomLevelText, zoomLevel);
            _settingsManager.EditorSettings.ZoomLevelMenuText = zoomLevelMenuText;

            _settingsManager.EditorSettings.IsZoom100Checked =
                Math.Abs(((_textBoxController.FontSize / fontSettings.FontSize) * 100) - 100) < 0.1;
        }

        public bool CanExecuteChooseFont()
        {
            return true;
        }

        public async void ExecuteChooseFont()
        {
            try
            {
                ShowFontChooserAndApplyChoiceToEditor();
            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError(e);
            }
        }

        public bool CanExecuteZoom100()
        {
            return !_settingsManager.EditorSettings.IsZoom100Checked;
        }

        public void ExecuteZoom100()
        {
            ApplyFontsFromSettingsToEditor();
            UpdateZoomLevelMenuText();
        }

        public bool CanExecuteZoomIn()
        {
            return _textBoxController.FontSize < 999;
        }

        public void ExecuteZoomIn()
        {
            _textBoxController.FontSize *= 1.05;
            UpdateZoomLevelMenuText();
        }


        public bool CanExecuteZoomOut()
        {
            return _textBoxController.FontSize > 1;
        }

        public void ExecuteZoomOut()
        {
            _textBoxController.FontSize *= 1 / 1.05;
            UpdateZoomLevelMenuText();
        }
    }
}
