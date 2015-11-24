using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VisualCrypt.Applications.Models.Settings;

namespace VisualCrypt.Droid.Models
{
    class FontSettings : IFontSettings
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}