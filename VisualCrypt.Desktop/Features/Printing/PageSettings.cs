using System.Runtime.Serialization;
using VisualCrypt.Desktop.Lib;

namespace VisualCrypt.Desktop.Models.Printing
{
    [DataContract]
    public class PageSettings : ViewModelBase
    {
        [DataMember]
        public int Margin
        {
            get { return _margin; }
            set
            {
                if (_margin == value) return;
                _margin = value;
                RaisePropertyChanged(() => Margin);
            }
        }
        int _margin;
    }
}
