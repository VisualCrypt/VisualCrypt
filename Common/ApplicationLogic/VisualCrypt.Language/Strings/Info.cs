using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace VisualCrypt.Language.Strings
{
    public class Info : INotifyPropertyChanged
    {
        public List<string> AvailableCultures = new List<string>() {"en", "de", "fr", "it", "ru" };

        public void SwitchCulture(string cultureString)
        {
            var culture = new CultureInfo(cultureString);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Resources.Culture = culture;
            OnCultureChanged();
            OnPropertyChanged(nameof(IsEN));
            OnPropertyChanged(nameof(IsDE));
            OnPropertyChanged(nameof(IsFR));
            OnPropertyChanged(nameof(IsIT));
            OnPropertyChanged(nameof(IsRU));
        }


        public bool IsEN { get { return IsCurrentCulture("en"); } }
        public bool IsDE { get { return IsCurrentCulture("de"); } }
        public bool IsFR { get { return IsCurrentCulture("fr"); } }
        public bool IsIT { get { return IsCurrentCulture("it"); } }
        public bool IsRU { get { return IsCurrentCulture("ru"); } }


        bool IsCurrentCulture(string cultureString)
        {
            if (CultureInfo.CurrentUICulture.Name.StartsWith(cultureString.ToLowerInvariant()))
                return true;
            return false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }


        public event EventHandler CultureChanged;
        void OnCultureChanged()
        {
            var handler = CultureChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
