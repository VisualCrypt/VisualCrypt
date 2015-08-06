//using System;
//using System.ComponentModel;
//using System.IO;
//using System.Runtime.CompilerServices;
//using System.Text;
//using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
//using VisualCrypt.Desktop.Shared.App;

//namespace VisualCrypt.Desktop.Shared.Files
//{
//	public class FileModel : INotifyPropertyChanged
//	{
//		static readonly Encoding VisualCryptTextSaveEncoding = new UTF8Encoding(false, true);
//		static readonly Encoding DefaultClearTextSaveEncoding = VisualCryptTextSaveEncoding;

//		FileModel()
//		{
//		}


//		public bool IsDirty
//		{
//			get { return _isDirty; }
//			set
//			{
//				if (_isDirty != value)
//				{
//					_isDirty = value;
//					OnPropertyChanged();
//				}
//			}
//		}

//		bool _isDirty;


//		public string Filename
//		{
//			get { return _filename; }
//			set
//			{
//				if (_filename != value)
//				{
//					_filename = value;
//					OnPropertyChanged();
//				}
//			}
//		}

//		string _filename;


//		public bool IsEncrypted
//		{
//			get { return _isEncrypted; }
//			private set
//			{
//				if (_isEncrypted != value)
//				{
//					_isEncrypted = value;
//					OnPropertyChanged();
//				}
//			}
//		}

//		bool _isEncrypted;


//		public string ClearTextContents { get; private set; }

//		public string VisualCryptText { get; private set; }

//		public Encoding SaveEncoding { get; private set; }

//		public CipherV2 CipherV2 { get; private set; }

//		public event PropertyChangedEventHandler PropertyChanged;

//		void OnPropertyChanged([CallerMemberName] string propertyName = null)
//		{
//			PropertyChangedEventHandler handler = PropertyChanged;
//			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
//		}

//		public bool CheckFilenameForQuickSave()
//		{
//			try
//			{
//				if (string.IsNullOrWhiteSpace(Filename) || Filename == Constants.UntitledDotVisualCrypt)
//					return false;
//				if (!Filename.EndsWith(Constants.DotVisualCrypt))
//					return false;
//				if (File.Exists(Filename))
//					return true;
//				return false;
//			}
//			catch (Exception)
//			{
//				return false;
//			}
//		}


//		public static FileModel EmptyCleartext()
//		{
//			return new FileModel
//			{
//				Filename = Constants.UntitledDotVisualCrypt,
//				ClearTextContents = string.Empty,
//				IsEncrypted = false,
//				SaveEncoding = DefaultClearTextSaveEncoding,
//				VisualCryptText = "SetError: VisualCryptText is not valid in this context."
//			};
//		}

//		public static FileModel Cleartext(string filename, string clearTextContents, Encoding saveEncoding)
//		{
//			return new FileModel
//			{
//				Filename = filename,
//				ClearTextContents = clearTextContents,
//				IsEncrypted = false,
//				SaveEncoding = saveEncoding,
//				VisualCryptText = "SetError: VisualCryptText is not valid in this context."
//			};
//		}

//		public static FileModel Encrypted(object cipherV2, string filename, string visualCryptText)
//		{
//			return new FileModel
//			{
//				Filename = filename,
//				ClearTextContents = "SetError: ClearTextContents is not valid in this context.",
//				IsEncrypted = true,
//				SaveEncoding = VisualCryptTextSaveEncoding,
//				VisualCryptText = visualCryptText,
//				CipherV2 = (CipherV2) cipherV2
//			};
//		}
//	}
//}