using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace VisualCrypt.Desktop.Shared.Files
{
	public class BindableFileInfo : INotifyPropertyChanged
	{
		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				_isDirty = value;
				OnPropertyChanged();
			}
		}

		bool _isDirty;

		public string Filename
		{
			get { return _filename; }
			set
			{
				_filename = value;
				OnPropertyChanged();
			}
		}

		string _filename;

		public bool IsEncrypted
		{
			get { return _isEncrypted; }
			set
			{
				_isEncrypted = value;
				if (_isEncrypted)
				{
					PlainTextBarVisibility = Visibility.Collapsed;
					EncryptedBarVisibility = Visibility.Visible;
					WorkingBarVisibility = Visibility.Collapsed;
				}
				else
				{
					PlainTextBarVisibility = Visibility.Visible;
					EncryptedBarVisibility = Visibility.Collapsed;
					WorkingBarVisibility = Visibility.Collapsed;
				}
				OnPropertyChanged();
			}
		}

		bool _isEncrypted;

		public Visibility PlainTextBarVisibility
		{
			get { return _plainTextBarVisibility; }
			set
			{
				if (_plainTextBarVisibility == value) return;
				_plainTextBarVisibility = value;
				OnPropertyChanged();
			}
		}
		Visibility _plainTextBarVisibility;

		public Visibility WorkingBarVisibility
		{
			get { return _workingBarVisibility; }
			set
			{
				if (_workingBarVisibility == value) return;
				_workingBarVisibility = value;
				if (_workingBarVisibility == Visibility.Visible)
				{
					PlainTextBarVisibility = Visibility.Collapsed;
					EncryptedBarVisibility = Visibility.Collapsed;
				}
				OnPropertyChanged();
			}
		}
		Visibility _workingBarVisibility;

		public Visibility EncryptedBarVisibility
		{
			get { return _encryptedBarVisibility; }
			set
			{
				if (_encryptedBarVisibility == value) return;
				_encryptedBarVisibility = value;
				OnPropertyChanged();
			}
		}
		Visibility _encryptedBarVisibility;

		public int ProgressPercent
		{
			get { return _progressPercent; }
			set
			{
				if (_progressPercent == value) return;
				_progressPercent = value;
				OnPropertyChanged();
			}
		}
		int _progressPercent;

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}