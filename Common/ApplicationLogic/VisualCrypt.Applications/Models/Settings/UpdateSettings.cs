using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace VisualCrypt.Applications.Models.Settings
{
	[DataContract]
	public class UpdateSettings : INotifyPropertyChanged
	{
		[DataMember]
		public string Version
		{
			get { return _version; }
			set
			{
				if (_version != value)
				{
					_version = value;
					OnPropertyChanged();
				}
			}
		}
		string _version;

        [DataMember]
        public string SKU
        {
            get { return _sku; }
            set
            {
                if (_sku != value)
                {
                    _sku = value;
                    OnPropertyChanged();
                }
            }
        }
        string _sku;

        [DataMember]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }
        DateTime _date;

        [DataMember]
        public bool Notify
        {
            get { return _notify; }
            set
            {
                if (_notify != value)
                {
                    _notify = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _notify;

        public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}