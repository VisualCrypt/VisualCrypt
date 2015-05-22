using VisualCrypt.Desktop.Lib;

namespace VisualCrypt.Desktop.Features.FindReplace
{
    public class SearchOptions : ViewModelBase
    {
        public bool MatchCase
        {
            get { return _matchCase;}
            set 
            {
                if (_matchCase == value) return;
                _matchCase = value;
                RaisePropertyChanged(() => MatchCase);
            }
        }
        bool _matchCase;

        public bool MatchWholeWord
        {
            get { return _matchWholeWord;}
            set 
            {
                if (_matchWholeWord == value) return;
                _matchWholeWord = value;
                RaisePropertyChanged(() => MatchWholeWord);
            }
        }
        bool _matchWholeWord;

        public bool SearchUp
        {
            get { return _searchUp;}
            set 
            {
                if (_searchUp == value) return;
                _searchUp = value;
                RaisePropertyChanged(() => SearchUp);
            }
        }
        bool _searchUp;

        public bool UseRegEx
        {
            get { return _useRegEx;}
            set 
            {
                if (_useRegEx == value) return;
                _useRegEx = value;
                RaisePropertyChanged(() => UseRegEx);
            }
        }
        bool _useRegEx;

       
    }
}
