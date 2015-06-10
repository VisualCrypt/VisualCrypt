using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using VisualCrypt.Desktop.Shared.App;

namespace VisualCrypt.Desktop.Shared.Files
{
	public class FileModel : INotifyPropertyChanged
	{
		static readonly Encoding VisualCryptTextSaveEncoding = new UTF8Encoding(false, true);
		static readonly Encoding DefaultClearTextSaveEncoding = VisualCryptTextSaveEncoding;

		FileModel()
		{
		}


		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				if (_isDirty != value)
				{
					_isDirty = value;
					OnPropertyChanged();
				}
			}
		}

		bool _isDirty;

		public string Filename { get; set; }

		public bool IsEncrypted { get; private set; }


		public string ClearTextContents { get; private set; }

		public string VisualCryptText { get; private set; }

		public Encoding SaveEncoding { get; private set; }

		public object CipherV2 { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool CheckFilenameForQuickSave()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(Filename) || Filename == Constants.UntitledDotVisualCrypt)
					return false;
				if (!Filename.EndsWith(Constants.DotVisualCrypt))
					return false;
				if (File.Exists(Filename))
					return true;
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}



		public static FileModel EmptyCleartext()
		{
			return new FileModel
			{
				Filename = Constants.UntitledDotVisualCrypt,
				ClearTextContents = string.Empty,
				IsEncrypted = false,
				SaveEncoding = DefaultClearTextSaveEncoding,
				VisualCryptText = "Error: VisualCryptText is not valid in this context."
			};
		}

		public static FileModel Cleartext(string filename, string clearTextContents, Encoding saveEncoding)
		{
			return new FileModel
			{
				Filename = filename,
				ClearTextContents = clearTextContents,
				IsEncrypted = false,
				SaveEncoding = saveEncoding,
				VisualCryptText = "Error: VisualCryptText is not valid in this context."
			};
		}

		public static FileModel Encrypted(object cipherV2, string filename, string visualCryptText)
		{
			return new FileModel
			{
				Filename = filename,
				ClearTextContents = "Error: ClearTextContents is not valid in this context.",
				IsEncrypted = true,
				SaveEncoding = VisualCryptTextSaveEncoding,
				VisualCryptText = visualCryptText,
				CipherV2 = cipherV2
			};
		}

		
	}
}