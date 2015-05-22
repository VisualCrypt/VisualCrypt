using System;
using System.Reflection;
using System.Windows.Controls;
using VisualCrypt.Desktop.Lib;

namespace VisualCrypt.Desktop.Views
{
    public sealed class GoToViewModel : ViewModelBase
    {
        readonly TextBox _textBox1;

        public IMessageBoxService MessageBoxService;
        public Action CloseAction;

        public GoToViewModel(TextBox textBox1)
        {
            _textBox1 = textBox1;
        }

        public string LineNo
        {
            get { return _lineNo; }
            set
            {
                if (_lineNo == value) return;
                _lineNo = value;
                RaisePropertyChanged(() => LineNo);

                GoCommand.RaiseCanExecuteChanged();
            }
        }
        string _lineNo;



        #region FindCommand

        private int _lineIndex;
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
                var lineLenght = _textBox1.GetLineLength(_lineIndex);
                SelectSearchResult(index, lineLenght);
                CloseAction();
            }
          
            catch (Exception e)
            {
                MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            }
        }

        void SelectSearchResult(int indexInSourceText, int lenght)
        {
            _textBox1.Select(indexInSourceText, lenght);
            _textBox1.Focus();
        }

        #endregion

    }
}