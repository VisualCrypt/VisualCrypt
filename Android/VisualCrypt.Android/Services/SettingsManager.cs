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
using VisualCrypt.Applications.Services.PortableImplementations;

namespace VisualCrypt.Droid.Services
{
    class SettingsManager : AbstractSettingsManager
    {
        string _currentDirectoryName;

        public override string CurrentDirectoryName
        {
            get { return _currentDirectoryName; }
            set { _currentDirectoryName = value; }
        }

        protected override void FactorySettings()
        {
            throw new NotImplementedException();
        }

        protected override string ReadSettingsFile()
        {
            throw new NotImplementedException();
        }

        protected override void WriteSettingsFile(string settingsFile)
        {
            throw new NotImplementedException();
        }
    }
}