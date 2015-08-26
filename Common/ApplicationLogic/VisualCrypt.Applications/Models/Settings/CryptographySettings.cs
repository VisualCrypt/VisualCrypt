using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace VisualCrypt.Applications.Models.Settings
{
	[DataContract]
	public class CryptographySettings : INotifyPropertyChanged
	{
		[DataMember]
		public byte LogRounds
		{
			get { return _logRounds; }
			set
			{
				if (_logRounds != value)
				{
					_logRounds = value;
					OnPropertyChanged();
				}
			}
		}

		byte _logRounds;

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}