using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace VisualCrypt.Applications.Apps.Settings
{
	/// <summary>
	/// Tracks the state of the editor. Some properties will be serialized, some not.
	/// </summary>
	[DataContract]
	public sealed class EditorSettings : INotifyPropertyChanged
	{
		[DataMember]
		public bool IsStatusBarChecked
		{
			get { return _isStatusBarChecked; }
			set
			{
				_isStatusBarChecked = value;
				OnPropertyChanged();
			}
		}

		bool _isStatusBarChecked;

		[DataMember]
		public bool IsToolAreaChecked
		{
			get { return _isToolAreaChecked; }
			set
			{
				_isToolAreaChecked = value;
				OnPropertyChanged();
			}
		}

		bool _isToolAreaChecked;


		[DataMember]
		public bool IsWordWrapChecked
		{
			get { return _isWordWrapChecked; }
			set
			{
				_isWordWrapChecked = value;
				OnPropertyChanged();
			}
		}

		bool _isWordWrapChecked;

		[DataMember]
		public bool IsSpellCheckingChecked
		{
			get { return _isSpellCheckingChecked; }
			set
			{
				_isSpellCheckingChecked = value;
				OnPropertyChanged();
			}
		}

		bool _isSpellCheckingChecked;

		[DataMember]
		public int PrintMargin
		{
			get { return _printMargin; }
			set
			{
				_printMargin = value;
				OnPropertyChanged();
			}
		}

		int _printMargin;

		

		public bool IsZoom100Checked
		{
			get { return _isZoom100Checked; }
			set
			{
				_isZoom100Checked = value;
				OnPropertyChanged();
			}
		}

		bool _isZoom100Checked;

		public string ZoomLevelMenuText
		{
			get { return _zoomLevelMenuText; }
			set
			{
				_zoomLevelMenuText = value;
				OnPropertyChanged();
			}
		}


		string _zoomLevelMenuText;

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}