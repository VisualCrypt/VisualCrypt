using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Desktop.ModuleEditor.Features.FindReplace;
using VisualCrypt.Desktop.ModuleEditor.Features.Printing;
using VisualCrypt.Desktop.Shared;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.Events;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.Services;
using VisualCrypt.Desktop.Shared.Settings;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
	[Export]
	public class EditorViewModel : ViewModelBase
	{
		TextBox _textBox1;
		TextBox _textboxFindFindString;
		TextBox _textbockReplaceFindString;
		TextBox _textboxReplaceString;
		TextBox _textBoxGotoString;
		UserControl _editorControl;
		SpellCheck _spellCheck;

		readonly IMessageBoxService _messageBoxService;
		readonly IEventAggregator _eventAggregator;

		[ImportingConstructor]
		public EditorViewModel(IEventAggregator eventAggregator, IMessageBoxService messageBoxService)
		{
			_eventAggregator = eventAggregator;
			_messageBoxService = messageBoxService;


			// From FRVM
			SearchOptions = new SearchOptions();
		}

		public void OnEditorInitialized(TextBox textbox1, TextBox textboxFindFindString, TextBox textbockReplaceFindString, TextBox textboxReplaceString, TextBox textBoxGotoString, UserControl editorControl)
		{
			_textBox1 = textbox1;
			_textboxFindFindString = textboxFindFindString;
			_textbockReplaceFindString = textbockReplaceFindString;
			_textboxReplaceString = textboxReplaceString;
			_textBoxGotoString = textBoxGotoString;
			_editorControl = editorControl;

			_spellCheck = _textBox1.SpellCheck;
			_spellCheck.SpellingReform = SpellingReform.Postreform;



			SettingsManager.EditorSettings.FontSettings.ApplyTo(_textBox1);
			ExecuteZoom100();

			OnTextReceived(string.Empty);

			_eventAggregator.GetEvent<EditorReceivesText>().Subscribe(OnTextReceived);
			_eventAggregator.GetEvent<EditorShouldSendText>().Subscribe(OnShouldSendText);
		}

		#region PubSubEvents

		void OnShouldSendText(Action<string> callback)
		{
			var args = new EditorSendsText { Text = _textBox1.Text, Callback = callback };
			_eventAggregator.GetEvent<EditorSendsText>().Publish(args);
		}

		void OnTextReceived(string newText)
		{
			if (newText == null)
				throw new ArgumentNullException("newText");

			_textBox1.IsUndoEnabled = false;

			var backup = FileManager.FileModel.IsDirty;
			_textBox1.Text = newText; // triggers Text_Changed which sets FileManager.FileModel.IsDirty = true;
			FileManager.FileModel.IsDirty = backup;

			if (FileManager.FileModel.IsEncrypted)
			{
				_textBox1.SpellCheck.IsEnabled = false;
				_textBox1.IsUndoEnabled = false;
				_textBox1.IsReadOnly = true;
			}
			else
			{
				_textBox1.SpellCheck.IsEnabled = SettingsManager.EditorSettings.IsSpellCheckingChecked;
				_textBox1.IsUndoEnabled = true;
				_textBox1.IsReadOnly = false;
			}
			UpdateStatusBar();
		}

		#endregion

		#region TextChangedCommand

		public void ExecuteTextChangedCommand()
		{
			FileManager.FileModel.IsDirty = true;
			UpdateStatusBar();
		}

		#endregion

		#region SelectionChangedCommand

		public void ExecuteSelectionChangedCommand()
		{
			if ((SettingsManager.EditorSettings.IsStatusBarChecked))
				UpdateStatusBar();
		}

		#endregion

		#region ZoomCommands

		public bool CanExecuteZoomIn()
		{
			return _textBox1.FontSize < 999;
		}

		public void ExecuteZoomIn()
		{
			_textBox1.FontSize *= 1.05;
			UpdateZoomLevelMenuText();

			//Zoom100Command.RaiseCanExecuteChanged();
			//ZoomInCommand.RaiseCanExecuteChanged();
			//ZoomOutCommand.RaiseCanExecuteChanged();
		}


		public bool CanExecuteZoomOut()
		{
			return _textBox1.FontSize > 1;
		}

		public void ExecuteZoomOut()
		{
			_textBox1.FontSize *= 1 / 1.05;
			UpdateZoomLevelMenuText();

			//Zoom100Command.RaiseCanExecuteChanged();
			//ZoomInCommand.RaiseCanExecuteChanged();
			//ZoomOutCommand.RaiseCanExecuteChanged();
		}


		public bool CanExecuteZoom100()
		{
			return !SettingsManager.EditorSettings.IsZoom100Checked;
		}

		public void ExecuteZoom100()
		{
			_textBox1.FontSize = SettingsManager.EditorSettings.FontSettings.FontSize;
			UpdateZoomLevelMenuText();

			//Zoom100Command.RaiseCanExecuteChanged();
			//ZoomInCommand.RaiseCanExecuteChanged();
			//ZoomOutCommand.RaiseCanExecuteChanged();
		}

		void UpdateZoomLevelMenuText()
		{
			var zoomLevel = (int)((_textBox1.FontSize / SettingsManager.EditorSettings.FontSettings.FontSize) * 100);
			var zoomLevelMenuText = "_Zoom (" + zoomLevel + "%)";
			SettingsManager.EditorSettings.ZoomLevelMenuText = zoomLevelMenuText;

			SettingsManager.EditorSettings.IsZoom100Checked =
				Math.Abs(((_textBox1.FontSize / SettingsManager.EditorSettings.FontSettings.FontSize) * 100) - 100) < 0.1;
		}

		#endregion

		#region PrintCommand

		public bool CanExecutePrint()
		{
			return true;
		}

		public void ExecutePrint()
		{
			try
			{
				var doc = Printer.ConvertToFlowDocument(_textBox1.Text);
				Printer.PrintFlowDocument(doc, SettingsManager.EditorSettings.PrintMargin);
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion


		#region TOOLAREA

		public int ToolAreaSelectedIndex
		{
			get { return _toolAreaSelectedIndex; }
			set
			{
				if (_toolAreaSelectedIndex != value)
				{
					_toolAreaSelectedIndex = value;
					OnPropertyChanged(() => ToolAreaSelectedIndex);
				}
			}
		}
		int _toolAreaSelectedIndex;

		public TabItem ToolAreaSelectedItem
		{
			get { return _toolAreaSelectedItem; }
			set
			{
				if (_toolAreaSelectedItem != value)
				{
					_toolAreaSelectedItem = value;
					OnPropertyChanged(() => ToolAreaSelectedItem);
				}
			}
		}
		TabItem _toolAreaSelectedItem;
		#endregion

		#region FindMenuCommand

		public bool CanExecuteFindMenuCommand()
		{
			return _textBox1.Text.Length > 0;
		}

		public void ExecuteFindMenuCommand()
		{
			try
			{
				SettingsManager.EditorSettings.IsToolAreaChecked = true;
				ToolAreaSelectedIndex = 0;
				_editorControl.UpdateLayout();
				_textboxFindFindString.SelectAll();
				_textboxFindFindString.Focus();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region FindNextMenuCommand

		public void ExecuteFindNextMenuCommand()
		{
			var oldDirection = SearchOptions.SearchUp;
			SearchOptions.SearchUp = false;
			FindNextCommand.Execute();
			SearchOptions.SearchUp = oldDirection;
		}

		public bool CanExecuteFindNextMenuCommand()
		{
			return FindNextCommand.CanExecute() &&
				   !string.IsNullOrEmpty(FindString);
		}

		#endregion

		#region FindPreviousMenuCommand

		public void ExecuteFindPreviousMenuCommand()
		{
			var oldDirection = SearchOptions.SearchUp;
			SearchOptions.SearchUp = true;
			FindNextCommand.Execute();
			SearchOptions.SearchUp = oldDirection;
		}

		public bool CanExecuteFindPreviousMenuCommand()
		{
			return FindNextCommand.CanExecute() &&
				   !string.IsNullOrEmpty(FindString);
		}

		#endregion

		#region FindLogic

		int _timesNotFound;

		public SearchOptions SearchOptions { get; set; }

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
				Find_SelectSearchResult(searchResult.Value.Index, searchResult.Value.Length);
			}

			if (!found && SearchOptions.UseRegEx == false)
				_messageBoxService.Show("'{0}' is not in the document.".FormatInvariant(FindString), "Find",
					MessageBoxButton.OK, MessageBoxImage.Exclamation);
			if (!found && SearchOptions.UseRegEx)
				_messageBoxService.Show(
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
				Find_SelectSearchResult(searchResult.Value.Index, ReplaceString.Length);
			}

			if (!found && SearchOptions.UseRegEx == false)
				_messageBoxService.Show("'{0}' could not be found.".FormatInvariant(FindString), "Replace",
					MessageBoxButton.OK, MessageBoxImage.Exclamation);
			if (!found && SearchOptions.UseRegEx)
				_messageBoxService.Show("No match for '{0}' could be found.".FormatInvariant(FindString), "Replace",
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


			_messageBoxService.Show("{0} occurrences were replaced.".FormatInvariant(count), "Replace All",
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
					_messageBoxService.Show(ae.Message, "Invalid Regular Expression Syntax", MessageBoxButton.OK,
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
						var result = _messageBoxService.Show(
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
							_messageBoxService.Show("Nothing found - Search again from the bottom of the document?",
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


		void Find_SelectSearchResult(int indexInSourceText, int length)
		{
			_textBox1.Select(indexInSourceText, length);
		}
		#endregion

		#region ReplaceCommand

		public bool CanExecuteReplace()
		{
			return _textBox1.Text.Length > 0;
		}


		public void ExecuteReplace()
		{

			SettingsManager.EditorSettings.IsToolAreaChecked = true;
			ToolAreaSelectedIndex = 1;
			_editorControl.UpdateLayout();
			_textbockReplaceFindString.SelectAll();
			_textbockReplaceFindString.Focus();

			// Find would focus textbox1 after found

		}

		#endregion

		#region DeleteLineCommand

		public bool CanExecuteDeleteLine()
		{
			return true;
		}

		public void ExecuteDeleteLine()
		{
			try
			{
				var currentLineIndex = GetCurrentLineIndex();

				// 1.) Get col in line to delete
				var currentColIndex = GetColIndex(currentLineIndex);

				// 2.) Is there a next line?
				var isNextLine = _textBox1.LineCount > currentLineIndex + 1;

				var currentLineLength = _textBox1.GetLineLength(currentLineIndex);
				if (currentLineLength < 1)
					return;


				var targetColIndex = 0;
				if (isNextLine)
				{
					var nextLineLength = _textBox1.GetLineLength(currentLineIndex + 1);
					var nextLineIsAtLeastAsLongAsCurrentLine = nextLineLength >= currentLineLength;

					if (nextLineIsAtLeastAsLongAsCurrentLine)
					{
						targetColIndex = currentColIndex;
					}
					else
					{
						targetColIndex = _textBox1.GetLineLength(currentLineIndex + 1) - 2;
						if (targetColIndex < 0)
							targetColIndex = 0;
					}
				}
				// targetColIndex stays 0 if there is no next line!


				var firstPos = _textBox1.GetCharacterIndexFromLineIndex(currentLineIndex);
				_textBox1.Text = _textBox1.Text.Remove(firstPos, currentLineLength);


				var newPos = _textBox1.GetCharacterIndexFromLineIndex(currentLineIndex) + targetColIndex;
				_textBox1.CaretIndex = newPos;
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region GoButtonCommand

		public string LineNo
		{
			get { return _lineNo; }
			set
			{
				if (_lineNo == value) return;
				_lineNo = value;
				OnPropertyChanged(() => LineNo);

				GoButtonCommand.RaiseCanExecuteChanged();
			}
		}
		string _lineNo;

		public int LineCount
		{
			get { return _textBox1.LineCount; }
		}


		public DelegateCommand GoButtonCommand
		{
			get { return CreateCommand(ref _goButtonCommand, ExecuteGoButtonCommand, CanExecuteGoButtonCommand); }
		}

		DelegateCommand _goButtonCommand;

		int _lineIndex;
		bool CanExecuteGoButtonCommand()
		{
			OnPropertyChanged(()=>LineCount);
			int lineNo;
			var canParse = int.TryParse(LineNo, out lineNo);
			if (!canParse)
				return false;
			if (lineNo <= 0)
				return false;
			if (_textBox1.LineCount < lineNo)
				return false;
			_lineIndex = lineNo - 1;
			return true;
		}

		void ExecuteGoButtonCommand()
		{
			try
			{
				var index = _textBox1.GetCharacterIndexFromLineIndex(_lineIndex);
				_textBox1.CaretIndex = index;
				var lineLength = _textBox1.GetLineLength(_lineIndex);
				GoButton_SelectSearchResult(index, lineLength);

			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		void GoButton_SelectSearchResult(int indexInSourceText, int length)
		{
			_textBox1.Select(indexInSourceText, length);
			_textBox1.Focus();
		}

		#endregion

		#region GoMenuCommand

		public void ExecuteGoMenuCommand()
		{
			try
			{
				SettingsManager.EditorSettings.IsToolAreaChecked = true;
				ToolAreaSelectedIndex = 2;

				_editorControl.UpdateLayout();
				_textBoxGotoString.SelectAll();
				_textBoxGotoString.Focus();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}

		}

		public bool CanExecuteGoMenuCommand()
		{
			return _textBox1.LineCount > 0;
		}

		#endregion

		#region DateCommand

		DateTime? _datePasted;

		public bool CanExecuteInsertDateTime()
		{
			return true;
		}

		public void ExecuteInsertDateTime()
		{
			if (_datePasted.HasValue && DateTime.Now - _datePasted.Value < TimeSpan.FromSeconds(3))
			{
				var oldCaretIndex = _textBox1.CaretIndex;
				var timeStringPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
				var timeString = " " + DateTime.Now.ToString(timeStringPattern);
				_textBox1.Text = _textBox1.Text.Insert(_textBox1.CaretIndex, timeString);
				_textBox1.CaretIndex = oldCaretIndex + 1 + timeString.Length;
				_datePasted = null;
			}
			else
			{
				var oldCaretIndex = _textBox1.CaretIndex;
				var datetringPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
				var dateString = DateTime.Now.ToString(datetringPattern);
				_textBox1.Text = _textBox1.Text.Insert(_textBox1.CaretIndex, dateString);
				_textBox1.CaretIndex = oldCaretIndex + dateString.Length;
				_datePasted = DateTime.Now;
			}
		}

		#endregion

		#region FontCommand

		public bool CanExecuteFont()
		{
			return true;
		}

		public async void ExecuteFont()
		{
			try
			{
				ParamsProvider.SetParams(typeof(Font), _textBox1.Text);
				var fontDialog = await WindowManager.GetDialogFromShowDialogAsyncWhenClosed<Font>();


				if (fontDialog.DialogResult == true)
				{
					ExecuteZoom100();
					Map.Copy(SettingsManager.EditorSettings.FontSettings, _textBox1);
					SettingsManager.SaveSettings();
				}
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}




		#endregion

		#region Private methods

		void UpdateStatusBar()
		{
			OnPropertyChanged(()=>LineCount);

			var pos = GetPositionString();
			var enc = GetEncodingString();

			var statusBarText = "{0} | {1} | Encrypted: {2}".FormatInvariant(enc, pos, FileManager.FileModel.IsEncrypted);
			_eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Publish(statusBarText);
		}

		static string GetEncodingString()
		{
			return FileManager.FileModel.SaveEncoding.EncodingName;
		}

		string GetPositionString()
		{
			var rawPos = _textBox1.CaretIndex;
			var lineIndex = GetCurrentLineIndex();
			var colIndex = GetColIndex(lineIndex);

			return "Ln {0}, Col {1} | Ch {2}/{3}".FormatInvariant(lineIndex + 1, colIndex + 1, rawPos,
				_textBox1.Text.Length);
		}

		int GetColIndex(int lineIndex)
		{
			var rawPos = _textBox1.CaretIndex;
			// get col and adjust for silly init values
			int colIndex;
			if (rawPos == 0 && lineIndex == 0)
				colIndex = 0;
			else
				colIndex = rawPos - _textBox1.GetCharacterIndexFromLineIndex(lineIndex);
			return colIndex;
		}

		int GetCurrentLineIndex()
		{
			var rawPos = _textBox1.CaretIndex;

			// get line and adjust for silly init values
			var lineIndex = _textBox1.GetLineIndexFromCharacterIndex(rawPos);
			if (lineIndex == -1)
				lineIndex = 0;
			return lineIndex;
		}

		#endregion
	}
}