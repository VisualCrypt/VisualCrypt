using System;
using System.IO;
using System.Windows;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Language;

namespace VisualCrypt.Desktop.Shared.Files
{
	public static class FileManager
	{
		public static readonly BindableFileInfo BindableFileInfo = new BindableFileInfo();
		static FileModel _fileModel = FileModel.EmptyCleartext();

		static FileManager()
		{
			Loc.LocaleChanged += delegate
			{
				if (_fileModel != null && _fileModel.CipherV2 != null && _fileModel.IsEncrypted)
					UpdateEncryptedBarText();
			};
		}


		public static FileModel FileModel
		{
			get { return _fileModel; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				_fileModel = value;
				_fileModel.PropertyChanged += _fileModel_PropertyChanged;
				BindableFileInfo.IsDirty = _fileModel.IsDirty;
				BindableFileInfo.Filename = Constants.ProductName + " - " + _fileModel.Filename;
				BindableFileInfo.IsEncrypted = _fileModel.IsEncrypted;
				if (_fileModel.IsEncrypted)
				{
					UpdateEncryptedBarText();
					ShowEncryptedBar();
				}
				else
					ShowPlainTextBar();
			}
		}
		public static bool CheckFilenameForQuickSave()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(_fileModel.Filename) || _fileModel.Filename == Loc.Strings.constUntitledDotVisualCrypt)
					return false;
				if (!_fileModel.Filename.EndsWith(Constants.DotVisualCrypt))
					return false;
				if (File.Exists(_fileModel.Filename))
					return true;
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		static void UpdateEncryptedBarText()
		{
			string text = string.Format(Loc.Strings.encrpytedStatusbarText, CipherV2.Version,
				_fileModel.CipherV2.RoundsExponent.Value, _fileModel.VisualCryptText.Length);
			BindableFileInfo.EncrytedBarText = text;
		}

		public static void ShowWorkingBar(string description)
		{
			BindableFileInfo.WorkingBarVisibility = Visibility.Visible;
			BindableFileInfo.PlainTextBarVisibility = Visibility.Collapsed;
			BindableFileInfo.EncryptedBarVisibility = Visibility.Collapsed;
			BindableFileInfo.ProgressBarOpName = description;
		}

		public static void ShowPlainTextBar()
		{
			BindableFileInfo.WorkingBarVisibility = Visibility.Collapsed;
			BindableFileInfo.PlainTextBarVisibility = Visibility.Visible;
			BindableFileInfo.EncryptedBarVisibility = Visibility.Collapsed;
		}

		public static void ShowEncryptedBar()
		{
			BindableFileInfo.WorkingBarVisibility = Visibility.Collapsed;
			BindableFileInfo.PlainTextBarVisibility = Visibility.Collapsed;
			BindableFileInfo.EncryptedBarVisibility = Visibility.Visible;
		}


		static void _fileModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var fileModel = (FileModel) sender;
			switch (e.PropertyName)
			{
				case "IsDirty":
					BindableFileInfo.IsDirty = fileModel.IsDirty;
					break;
				case "Filename":
					BindableFileInfo.Filename = fileModel.Filename;
					break;
				case "IsEncrypted":
					BindableFileInfo.IsEncrypted = fileModel.IsEncrypted;
					break;
				default:
					throw new ArgumentException("Unknown property name {0}.", e.PropertyName);
			}
		}
	}
}