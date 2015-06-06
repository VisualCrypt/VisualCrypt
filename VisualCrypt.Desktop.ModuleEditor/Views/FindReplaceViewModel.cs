﻿using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using VisualCrypt.Desktop.ModuleEditor.Features.FindReplace;
using VisualCrypt.Desktop.Shared;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
	[Export]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public sealed class FindReplaceViewModel : ViewModelBase
	{
		TextBox _textBox1;

		int _timesNotFound;

		public void SetTextBox(TextBox textBox1)
		{
			_textBox1 = textBox1;
		}

		public SearchOptions SearchOptions { get; set; }

		public IMessageBoxService MessageBoxService;

		public FindReplaceViewModel()
		{
			SearchOptions = new SearchOptions();
		}

		int Pos
		{
			get { return _textBox1.CaretIndex; }
			set { _textBox1.CaretIndex = value; }
		}


		public string FindString
		{
			get { return _findString; }
			set
			{
				if (_findString == value) return;
				_findString = value;
				OnPropertyChanged(() => FindString);

				FindNextCommand.RaiseCanExecuteChanged();
				ReplaceCommand.RaiseCanExecuteChanged();
				ReplaceAllCommand.RaiseCanExecuteChanged();
			}
		}

		string _findString = string.Empty;

		public string ReplaceString
		{
			get { return _replaceString; }
			set
			{
				if (_replaceString == value) return;
				_replaceString = value;
				OnPropertyChanged(() => ReplaceString);
			}
		}

		string _replaceString = string.Empty;

		public int TabControlSelectedIndex
		{
			get { return _tabControlSelectedIndex; }
			set
			{
				if (_tabControlSelectedIndex == value) return;
				_tabControlSelectedIndex = value;
				OnPropertyChanged(() => TabControlSelectedIndex);
			}
		}

		int _tabControlSelectedIndex;

		#region FindCommand

		public DelegateCommand FindNextCommand
		{
			get
			{
				return CreateCommand(ref _findNextCommand, ExecuteFindNextCommand,
					() => !string.IsNullOrEmpty(FindString));
			}
		}

		DelegateCommand _findNextCommand;


		void ExecuteFindNextCommand()
		{
			var found = false;
			if (!SearchOptions.SearchUp)
				Pos = Pos + _textBox1.SelectionLength;

			var searchResult = Find(true, false);

			if (searchResult.HasValue)
			{
				found = true;
				SelectSearchResult(searchResult.Value.Index, searchResult.Value.Length);
			}

			if (!found && SearchOptions.UseRegEx == false)
				MessageBoxService.Show("'{0}' is not in the document.".FormatInvariant(FindString), "Find",
					MessageBoxButton.OK, MessageBoxImage.Exclamation);
			if (!found && SearchOptions.UseRegEx)
				MessageBoxService.Show(
					"The expression '{0}' yields no matches in the document.".FormatInvariant(FindString), "Find",
					MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		SearchResult? Find(bool wrapSearchPos, bool isReplace)
		{
			_timesNotFound = 0;
			return SearchRecoursive(wrapSearchPos, isReplace);
		}

		#endregion

		#region ReplaceCommand

		public DelegateCommand ReplaceCommand
		{
			get { return CreateCommand(ref _replaceCommand, ExecuteReplaceCommand, () => !string.IsNullOrEmpty(FindString)); }
		}

		DelegateCommand _replaceCommand;


		void ExecuteReplaceCommand()
		{
			var found = false;
			if (!SearchOptions.SearchUp)
				Pos = Pos + _textBox1.SelectionLength;

			var searchResult = Find(true, true);

			if (searchResult.HasValue)
			{
				var removed = _textBox1.Text.Remove(searchResult.Value.Index, searchResult.Value.Length);
				_textBox1.Text = removed.Insert(searchResult.Value.Index, ReplaceString);

				found = true;
				SelectSearchResult(searchResult.Value.Index, ReplaceString.Length);
			}

			if (!found && SearchOptions.UseRegEx == false)
				MessageBoxService.Show("'{0}' could not be found.".FormatInvariant(FindString), "Replace",
					MessageBoxButton.OK, MessageBoxImage.Exclamation);
			if (!found && SearchOptions.UseRegEx)
				MessageBoxService.Show("No match for '{0}' could be found.".FormatInvariant(FindString), "Replace",
					MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		#endregion

		#region ReplaceAllCommand

		public DelegateCommand ReplaceAllCommand
		{
			get
			{
				return CreateCommand(ref _replaceAllCommand, ExecuteReplaceAllCommand,
					() => !string.IsNullOrEmpty(FindString));
			}
		}

		DelegateCommand _replaceAllCommand;


		void ExecuteReplaceAllCommand()
		{
			SearchOptions.SearchUp = false;
			Pos = 0;
			var count = 0;

			start:
			var searchResult = Find(false, false);

			if (searchResult.HasValue)
			{
				var removed = _textBox1.Text.Remove(searchResult.Value.Index, searchResult.Value.Length);
				_textBox1.Text = removed.Insert(searchResult.Value.Index, ReplaceString);
				count++;
				Pos = searchResult.Value.Index + ReplaceString.Length;

				goto start;
			}
			var image = (count > 0) ? MessageBoxImage.Information : MessageBoxImage.Exclamation;


			MessageBoxService.Show("{0} occurrences were replaced.".FormatInvariant(count), "Replace All",
				MessageBoxButton.OK, image);
		}

		#endregion

		SearchResult? SearchRecoursive(bool wrapSearchPos, bool isReplace)
		{
			SearchResult? searchResult;
			try
			{
				searchResult = SearchStrategy.Search(_textBox1.Text, _findString, Pos, SearchOptions);
			}
			catch (ArgumentException ae)
			{
				if (SearchOptions.UseRegEx)
				{
					MessageBoxService.Show(ae.Message, "Invalid Regular Expression Syntax", MessageBoxButton.OK,
						MessageBoxImage.Error);
					return null;
				}
				throw;
			}
			if (searchResult.HasValue)
			{
				return searchResult;
			}
			// if not found..
			if (!wrapSearchPos)
				return null;

			_timesNotFound += 1;
			if (_timesNotFound < 2)
			{
				if (!SearchOptions.SearchUp)
				{
					if (isReplace && Pos != 0)
					{
						var result = MessageBoxService.Show(
							"Nothing found - Search again from the top of the document?", "Replace",
							MessageBoxButton.OKCancel, MessageBoxImage.Question);
						if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
							return null;
					}
					Pos = 0;
				}
				else
				{
					if (isReplace && Pos != _textBox1.Text.Length)
					{
						var result =
							MessageBoxService.Show("Nothing found - Search again from the bottom of the document?",
								"Replace",
								MessageBoxButton.OKCancel, MessageBoxImage.Question);
						if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
							return null;
					}
					Pos = _textBox1.Text.Length;
				}

				return SearchRecoursive(true, isReplace);
			}
			_timesNotFound = 0;
			return null;
		}


		void SelectSearchResult(int indexInSourceText, int length)
		{
			_textBox1.Select(indexInSourceText, length);
		}
	}
}