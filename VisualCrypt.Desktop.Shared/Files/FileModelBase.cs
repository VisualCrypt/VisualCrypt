using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace VisualCrypt.Desktop.Shared.Files
{
	public class FileModelBase : INotifyPropertyChanged
	{
		public virtual bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				if (true)
				{
					_isDirty = value;
					OnPropertyChanged();
				}
			}
		}

		bool _isDirty;

		public string Filename
		{
			get { return _filename; }
			protected set
			{
				if (_filename != value)
				{
					_filename = value;
					OnPropertyChanged();
				}
			}
		}

		string _filename;

		public bool IsEncrypted
		{
			get { return _isEncrypted; }
			protected set
			{
				if (_isEncrypted != value)
				{
					_isEncrypted = value;
					OnPropertyChanged();
				}
			}
		}

		bool _isEncrypted;


		public string Contents { get; protected set; }

		public Encoding SaveEncoding { get; protected set; }


		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}