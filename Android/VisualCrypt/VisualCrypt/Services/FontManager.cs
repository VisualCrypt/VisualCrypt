using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Droid.Services
{
    class FontManager : IFontManager
    {
        public void ApplyFontsFromSettingsToEditor()
        {
            
        }

        public bool CanExecuteChooseFont()
        {
            return false;
        }

        public bool CanExecuteZoom100()
        {
            return false;
        }

        public bool CanExecuteZoomIn()
        {
            return false;
        }

        public bool CanExecuteZoomOut()
        {
            return false;
        }

        public void ExecuteChooseFont()
        {
            throw new NotImplementedException();
        }

        public void ExecuteZoom100()
        {
            
        }

        public void ExecuteZoomIn()
        {
            throw new NotImplementedException();
        }

        public void ExecuteZoomOut()
        {
            throw new NotImplementedException();
        }

        public void ShowFontChooserAndApplyChoiceToEditor()
        {
            throw new NotImplementedException();
        }
    }
}