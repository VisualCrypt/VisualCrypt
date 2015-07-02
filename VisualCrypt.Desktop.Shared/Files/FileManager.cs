﻿using System;
using System.Windows;
using VisualCrypt.Desktop.Shared.App;

namespace VisualCrypt.Desktop.Shared.Files
{
	public static class FileManager
	{
		public static readonly BindableFileInfo BindableFileInfo = new BindableFileInfo();
		static FileModel _fileModel = FileModel.EmptyCleartext();


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
					ShowEncryptedBar();
				else 
					ShowPlainTextBar();
			}
		}

		public static void ShowWorkingBar(string description)
		{
			BindableFileInfo.WorkingBarVisibility = Visibility.Visible;
			BindableFileInfo.PlainTextBarVisibility = Visibility.Collapsed;
			BindableFileInfo.EncryptedBarVisibility = Visibility.Collapsed;
			BindableFileInfo.ProgressBarText = description;
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