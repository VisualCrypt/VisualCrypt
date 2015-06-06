using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using VisualCrypt.Desktop.Shared;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
	[Export]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public sealed class GoToViewModel : ViewModelBase
	{
		TextBox _textBox1;

		public IMessageBoxService MessageBoxService;
		public Action CloseAction;

		public string LineNo
		{
			get { return _lineNo; }
			set
			{
				if (_lineNo == value) return;
				_lineNo = value;
				OnPropertyChanged(() => LineNo);

				GoCommand.RaiseCanExecuteChanged();
			}
		}

		string _lineNo;

		#region GoCommand

		int _lineIndex;

		public DelegateCommand GoCommand
		{
			get { return CreateCommand(ref _goCommand, ExecuteGoCommand, CanExecuteGoToCommand); }
		}


		DelegateCommand _goCommand;

		bool CanExecuteGoToCommand()
		{
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

		void ExecuteGoCommand()
		{
			try
			{
				//_textBox1.ScrollToLine(_lineIndex);

				var index = _textBox1.GetCharacterIndexFromLineIndex(_lineIndex);
				_textBox1.CaretIndex = index;
				var lineLength = _textBox1.GetLineLength(_lineIndex);
				SelectSearchResult(index, lineLength);
				CloseAction();
			}

			catch (Exception e)
			{
				MessageBoxService.ShowError(e);
			}
		}

		void SelectSearchResult(int indexInSourceText, int length)
		{
			_textBox1.Select(indexInSourceText, length);
			_textBox1.Focus();
		}

		#endregion

		public void SetTextBox(TextBox textBox1)
		{
			_textBox1 = textBox1;
		}
	}
}