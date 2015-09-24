using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace VisualCrypt.Applications.Models.Settings
{
	/// <summary>
	/// Tracks the state of the editor. Some properties will be serialized, some not.
	/// </summary>
	[DataContract]
	public sealed class EditorSettings : INotifyPropertyChanged
	{
		

		[DataMember]
		public bool IsToolAreaVisible
		{
			get { return _isToolAreaVisible; }
			set
			{
				_isToolAreaVisible = value;
				OnPropertyChanged();
			}
		}

		bool _isToolAreaVisible;


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
		public int PagePadding
		{
			get { return _pagePadding; }
			set
			{
				_pagePadding = value;
				OnPropertyChanged();
			}
		}

		int _pagePadding;

		

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