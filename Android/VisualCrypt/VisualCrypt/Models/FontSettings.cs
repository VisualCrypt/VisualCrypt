using System.ComponentModel;
using VisualCrypt.Applications.Models.Settings;
using System.Runtime.Serialization;

namespace VisualCrypt.Droid.Models
{
    [DataContract]
    public class FontSettings : IFontSettings
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}