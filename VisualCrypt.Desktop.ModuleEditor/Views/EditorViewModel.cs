using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Desktop.ModuleEditor.FeatureSupport.FindReplace;
using VisualCrypt.Desktop.ModuleEditor.FeatureSupport.Printing;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.Events;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.PrismSupport;
using VisualCrypt.Desktop.Shared.Services;
using VisualCrypt.Desktop.Shared.Settings;
using VisualCrypt.Language;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
	[Export]
	public class EditorViewModel : ViewModelBase
	{
		#region Initialization

		IEditor _editor;

		public IMessageBoxService _messageBoxService;
		readonly IEventAggregator _eventAggregator;

		[ImportingConstructor]
		public EditorViewModel(IEventAggregator eventAggregator, IMessageBoxService messageBoxService)
		{
			_eventAggregator = eventAggregator;
			_messageBoxService = messageBoxService;
		}

		public void OnEditorLoaded(IEditor editor)
		{
			_editor = editor;
			SettingsManager.EditorSettings.FontSettings.ApplyTo(_editor.TextBox1);
			SearchOptions = new SearchOptions();
			ExecuteZoom100();
			Loc.LocaleChanged += delegate
			{
				UpdateZoomLevelMenuText();
				UpdateStatusBar();
			};


			_editor.TextBox1.TextChanged += OnTextChanged;
			_editor.TextBox1.SelectionChanged += OnSelectionChanged;
			_eventAggregator.GetEvent<EditorReceivesText>().Subscribe(OnTextReceived);
			_eventAggregator.GetEvent<EditorShouldSendText>().Subscribe(OnTextRequested);

			OnTextReceived(string.Empty);
		}

		#endregion

		#region Events

		void OnTextRequested(Action<string> callback)
		{
			var args = new EditorSendsText { Text = _editor.TextBox1.Text, Callback = callback };
			_eventAggregator.GetEvent<EditorSendsText>().Publish(args);
		}


		void OnTextReceived(string newText)
		{
			if (newText == null)
				throw new ArgumentNullException("newText");

			_editor.TextBox1.IsUndoEnabled = false;

			var backup = FileManager.FileModel.IsDirty;
			_editor.TextBox1.Text = newText; // triggers Text_Changed which sets FileManager.FileModel.IsDirty = true;
			FileManager.FileModel.IsDirty = backup;

			if (FileManager.FileModel.IsEncrypted)
			{
				_editor.TextBox1.IsUndoEnabled = false;
				_editor.TextBox1.IsReadOnly = true;
			}
			else
			{
				if (FileManager.FileModel.SaveEncoding != null)
				{
					_editor.TextBox1.IsUndoEnabled = true;
					_editor.TextBox1.IsReadOnly = false;
				}
				else
				{
					SettingsManager.EditorSettings.IsWordWrapChecked = false;
					_editor.TextBox1.IsUndoEnabled = false;
					_editor.TextBox1.IsReadOnly = true;
				}

			}
			UpdateStatusBar();
		}

		void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			FileManager.FileModel.IsDirty = true;
			UpdateStatusBar();
		}

		void OnSelectionChanged(object sender, RoutedEventArgs routedEventArgs)
		{
			if ((SettingsManager.EditorSettings.IsStatusBarChecked))
				UpdateStatusBar();
		}

		#endregion

		#region ZoomCommands

		public bool CanExecuteZoomIn()
		{
			return _editor.TextBox1.FontSize < 999;
		}

		public void ExecuteZoomIn()
		{
			_editor.TextBox1.FontSize *= 1.05;
			UpdateZoomLevelMenuText();

			//Zoom100Command.RaiseCanExecuteChanged();
			//ZoomInCommand.RaiseCanExecuteChanged();
			//ZoomOutCommand.RaiseCanExecuteChanged();
		}


		public bool CanExecuteZoomOut()
		{
			return _editor.TextBox1.FontSize > 1;
		}

		public void ExecuteZoomOut()
		{
			_editor.TextBox1.FontSize *= 1 / 1.05;
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
			_editor.TextBox1.FontSize = SettingsManager.EditorSettings.FontSettings.FontSize;
			UpdateZoomLevelMenuText();

			//Zoom100Command.RaiseCanExecuteChanged();
			//ZoomInCommand.RaiseCanExecuteChanged();
			//ZoomOutCommand.RaiseCanExecuteChanged();
		}

		void UpdateZoomLevelMenuText()
		{
			if(_editor == null) // this method is hooked to the LocalChanged event and may be called any time
				return;

			var zoomLevel = (int)((_editor.TextBox1.FontSize / SettingsManager.EditorSettings.FontSettings.FontSize) * 100);
			var zoomLevelMenuText = string.Format(Loc.Strings.miViewZoomLevelText, zoomLevel);
			SettingsManager.EditorSettings.ZoomLevelMenuText = zoomLevelMenuText;

			SettingsManager.EditorSettings.IsZoom100Checked =
				Math.Abs(((_editor.TextBox1.FontSize / SettingsManager.EditorSettings.FontSettings.FontSize) * 100) - 100) < 0.1;
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
				var doc = Printer.ConvertToFlowDocument(_editor.TextBox1.Text);
				Printer.PrintFlowDocument(doc, SettingsManager.EditorSettings.PrintMargin);
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region CloseToolAreaCommand

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

		public DelegateCommand CloseToolAreaCommand
		{
			get
			{
				return CreateCommand(ref _closeToolAreaCommand, () => SettingsManager.EditorSettings.IsToolAreaChecked = false,
					() => true);
			}
		}
		DelegateCommand _closeToolAreaCommand;
		#endregion

		#region FindMenuCommand

		public bool CanExecuteFindMenuCommand()
		{
			return _editor.TextBox1.Text.Length > 0;
		}

		public void ExecuteFindMenuCommand()
		{
			try
			{
				SettingsManager.EditorSettings.IsToolAreaChecked = true;
				ToolAreaSelectedIndex = 0;
				_editor.EditorControl.UpdateLayout();
				_editor.TextBoxFind.SelectAll();
				_editor.TextBoxFind.Focus();
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
			FindNextButtonCommand.Execute();
			SearchOptions.SearchUp = oldDirection;
		}

		public bool CanExecuteFindNextMenuCommand()
		{
			return FindNextButtonCommand.CanExecute() &&
				   !string.IsNullOrEmpty(FindString);
		}

		#endregion

		#region FindPreviousMenuCommand

		public void ExecuteFindPreviousMenuCommand()
		{
			var oldDirection = SearchOptions.SearchUp;
			SearchOptions.SearchUp = true;
			FindNextButtonCommand.Execute();
			SearchOptions.SearchUp = oldDirection;
		}

		public bool CanExecuteFindPreviousMenuCommand()
		{
			return FindNextButtonCommand.CanExecute() &&
				   !string.IsNullOrEmpty(FindString);
		}

		#endregion

		#region ReplaceMenuCommand

		public bool CanExecuteReplaceMenuCommand()
		{
			return _editor.TextBox1.Text.Length > 0 && FileManager.FileModel.SaveEncoding != null;
		}

		public void ExecuteReplaceMenuCommand()
		{
			SettingsManager.EditorSettings.IsToolAreaChecked = true;
			ToolAreaSelectedIndex = 1;
			_editor.EditorControl.UpdateLayout();
			_editor.TextBoxFindReplace.SelectAll();
			_editor.TextBoxFindReplace.Focus();
		}

		#endregion

		#region FindLogic

		int _timesNotFound;

		public SearchOptions SearchOptions { get; set; }

		int Pos
		{
			get { return _editor.TextBox1.CaretIndex; }
			set { _editor.TextBox1.CaretIndex = value; }
		}


		public string FindString
		{
			get { return _findString; }
			set
			{
				if (_findString == value) return;
				_findString = value;
				OnPropertyChanged(() => FindString);

				FindNextButtonCommand.RaiseCanExecuteChanged();
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

		#region FindNextButtonCommand

		public DelegateCommand FindNextButtonCommand
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
				Pos = Pos + _editor.TextBox1.SelectionLength;

			var searchResult = Find(true, false);

			if (searchResult.HasValue)
			{
				found = true;
				SelectSearchResult(searchResult.Value.Index, searchResult.Value.Length);
			}

			if (!found && SearchOptions.UseRegEx == false)
				_messageBoxService.Show(string.Format("'{0}' is not in the document.", FindString), "Find",
					MessageBoxButton.OK, MessageBoxImage.Exclamation);
			if (!found && SearchOptions.UseRegEx)
				_messageBoxService.Show(
					string.Format("The expression '{0}' yields no matches in the document.", FindString), "Find",
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
				Pos = Pos + _editor.TextBox1.SelectionLength;

			var searchResult = Find(true, true);

			if (searchResult.HasValue)
			{
				var removed = _editor.TextBox1.Text.Remove(searchResult.Value.Index, searchResult.Value.Length);
				_editor.TextBox1.Text = removed.Insert(searchResult.Value.Index, ReplaceString);

				found = true;
				SelectSearchResult(searchResult.Value.Index, ReplaceString.Length);
			}

			if (!found && SearchOptions.UseRegEx == false)
				_messageBoxService.Show(string.Format("'{0}' could not be found.", FindString), "Replace",
					MessageBoxButton.OK, MessageBoxImage.Exclamation);
			if (!found && SearchOptions.UseRegEx)
				_messageBoxService.Show(string.Format("No match for '{0}' could be found.", FindString), "Replace",
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
				var removed = _editor.TextBox1.Text.Remove(searchResult.Value.Index, searchResult.Value.Length);
				_editor.TextBox1.Text = removed.Insert(searchResult.Value.Index, ReplaceString);
				count++;
				Pos = searchResult.Value.Index + ReplaceString.Length;

				goto start;
			}
			var image = (count > 0) ? MessageBoxImage.Information : MessageBoxImage.Exclamation;


			_messageBoxService.Show(string.Format("{0} occurrences were replaced.", count), "Replace All",
				MessageBoxButton.OK, image);
		}

		#endregion

		SearchResult? SearchRecoursive(bool wrapSearchPos, bool isReplace)
		{
			SearchResult? searchResult;
			try
			{
				searchResult = SearchStrategy.Search(_editor.TextBox1.Text, _findString, Pos, SearchOptions);
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
					if (isReplace && Pos != _editor.TextBox1.Text.Length)
					{
						var result =
							_messageBoxService.Show("Nothing found - Search again from the bottom of the document?",
								"Replace",
								MessageBoxButton.OKCancel, MessageBoxImage.Question);
						if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
							return null;
					}
					Pos = _editor.TextBox1.Text.Length;
				}

				return SearchRecoursive(true, isReplace);
			}
			_timesNotFound = 0;
			return null;
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
				var isNextLine = _editor.TextBox1.LineCount > currentLineIndex + 1;

				var currentLineLength = _editor.TextBox1.GetLineLength(currentLineIndex);
				if (currentLineLength < 1)
					return;


				var targetColIndex = 0;
				if (isNextLine)
				{
					var nextLineLength = _editor.TextBox1.GetLineLength(currentLineIndex + 1);
					var nextLineIsAtLeastAsLongAsCurrentLine = nextLineLength >= currentLineLength;

					if (nextLineIsAtLeastAsLongAsCurrentLine)
					{
						targetColIndex = currentColIndex;
					}
					else
					{
						targetColIndex = _editor.TextBox1.GetLineLength(currentLineIndex + 1) - 2;
						if (targetColIndex < 0)
							targetColIndex = 0;
					}
				}
				// targetColIndex stays 0 if there is no next line!


				var firstPos = _editor.TextBox1.GetCharacterIndexFromLineIndex(currentLineIndex);
				_editor.TextBox1.Text = _editor.TextBox1.Text.Remove(firstPos, currentLineLength);


				var newPos = _editor.TextBox1.GetCharacterIndexFromLineIndex(currentLineIndex) + targetColIndex;
				_editor.TextBox1.CaretIndex = newPos;
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
			get
			{
				if (_editor != null)
					return _editor.TextBox1.LineCount;
				return 0;
			}
		}


		public DelegateCommand GoButtonCommand
		{
			get { return CreateCommand(ref _goButtonCommand, ExecuteGoButtonCommand, CanExecuteGoButtonCommand); }
		}

		DelegateCommand _goButtonCommand;

		int _lineIndex;

		bool CanExecuteGoButtonCommand()
		{
			OnPropertyChanged(() => LineCount);
			int lineNo;
			var canParse = int.TryParse(LineNo, out lineNo);
			if (!canParse)
				return false;
			if (lineNo <= 0)
				return false;
			if (_editor.TextBox1.LineCount < lineNo)
				return false;
			_lineIndex = lineNo - 1;
			return true;
		}

		void ExecuteGoButtonCommand()
		{
			try
			{
				var index = _editor.TextBox1.GetCharacterIndexFromLineIndex(_lineIndex);
				_editor.TextBox1.CaretIndex = index;
				var lineLength = _editor.TextBox1.GetLineLength(_lineIndex);
				SelectSearchResult(index, lineLength);
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		void SelectSearchResult(int indexInSourceText, int length)
		{
			_editor.TextBox1.Select(indexInSourceText, length);
			_editor.TextBox1.Focus();
		}

		#endregion

		#region GoMenuCommand

		public void ExecuteGoMenuCommand()
		{
			try
			{
				SettingsManager.EditorSettings.IsToolAreaChecked = true;
				ToolAreaSelectedIndex = 2;
				_editor.EditorControl.UpdateLayout();
				_editor.TextBoxGoTo.SelectAll();
				_editor.TextBoxGoTo.Focus();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		public bool CanExecuteGoMenuCommand()
		{
			return _editor.TextBox1.LineCount > 0;
		}

		#endregion

		#region DateCommand

		DateTime? _datePasted;

		public bool CanExecuteInsertDateTime()
		{
			return FileManager.FileModel.SaveEncoding != null;
		}

		public void ExecuteInsertDateTime()
		{
			if (_datePasted.HasValue && DateTime.Now - _datePasted.Value < TimeSpan.FromSeconds(3))
			{
				var oldCaretIndex = _editor.TextBox1.CaretIndex;
				var timeStringPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
				var timeString = " " + DateTime.Now.ToString(timeStringPattern);
				_editor.TextBox1.Text = _editor.TextBox1.Text.Insert(_editor.TextBox1.CaretIndex, timeString);
				_editor.TextBox1.CaretIndex = oldCaretIndex + 1 + timeString.Length;
				_datePasted = null;
			}
			else
			{
				var oldCaretIndex = _editor.TextBox1.CaretIndex;
				var datetringPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
				var dateString = DateTime.Now.ToString(datetringPattern);
				_editor.TextBox1.Text = _editor.TextBox1.Text.Insert(_editor.TextBox1.CaretIndex, dateString);
				_editor.TextBox1.CaretIndex = oldCaretIndex + dateString.Length;
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
				ParamsProvider.SetParams(typeof(Font), _editor.TextBox1.Text);
				var fontDialog = await WindowManager.GetDialogFromShowDialogAsyncWhenClosed<Font>();


				if (fontDialog.DialogResult == true)
				{
					ExecuteZoom100();
					Map.Copy(SettingsManager.EditorSettings.FontSettings, _editor.TextBox1);
					SettingsManager.SaveSettings(SettingsManager.EditorSettings.FontSettings);
				}
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region Private methods

		internal void UpdateStatusBar()
		{
			OnPropertyChanged(() => LineCount);

			var pos = GetPositionString();
			var enc = GetEncodingString();

			var statusBarText = string.Format(CultureInfo.InvariantCulture, Loc.Strings.plaintextStatusbarText, pos, enc);
			_eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Publish(statusBarText);
		}

		static string GetEncodingString()
		{
			return FileManager.FileModel.SaveEncoding != null ? FileManager.FileModel.SaveEncoding.EncodingName : "Hex View";
		}

		string GetPositionString()
		{
			var rawPos = _editor.TextBox1.CaretIndex;
			var lineIndex = GetCurrentLineIndex();
			var colIndex = GetColIndex(lineIndex);

			return string.Format(CultureInfo.InvariantCulture, Loc.Strings.plaintextStatusbarPositionInfo, lineIndex + 1, colIndex + 1, rawPos,
				_editor.TextBox1.Text.Length);
		}

		int GetColIndex(int lineIndex)
		{
			var rawPos = _editor.TextBox1.CaretIndex;
			// get col and adjust for silly init values
			int colIndex;
			if (rawPos == 0 && lineIndex == 0)
				colIndex = 0;
			else
				colIndex = rawPos - _editor.TextBox1.GetCharacterIndexFromLineIndex(lineIndex);
			return colIndex;
		}

		int GetCurrentLineIndex()
		{
			var rawPos = _editor.TextBox1.CaretIndex;

			// get line and adjust for silly init values
			var lineIndex = _editor.TextBox1.GetLineIndexFromCharacterIndex(rawPos);
			if (lineIndex == -1)
				lineIndex = 0;
			return lineIndex;
		}

		#endregion
	}
}