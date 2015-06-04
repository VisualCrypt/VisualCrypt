﻿using System.Windows;
using System.Windows.Input;
using VisualCrypt.Desktop.Shared;

namespace VisualCrypt.Desktop.Views
{
	public partial class AboutDialog
	{
		public AboutDialog()
		{
			InitializeComponent();
			DataContext = this;
			PreviewKeyDown += CloseWithEscape;
		}

		void CloseWithEscape(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();
		}

		public static string NotepadVersion
		{
			get { return Version.NotepadVersion; }
		}


		void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}