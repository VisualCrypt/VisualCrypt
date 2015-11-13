using System.ComponentModel;

namespace VisualCrypt.Language.Strings
{
    /// <summary>
    /// How to update/add a Resource:
    /// 1. Enter the new string in the Resource designer.
    /// 2. Build the VisualCrypt.Applications project. This will trigger to run BuildTools, which generates the necessary code.
    /// </summary>
    public partial class ResourceWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Info Info { get { return _info; } }
        readonly Info _info;
        public ResourceWrapper()
        {
            _info = new Info();
            Info.CultureChanged += (s, e) =>
            {
                var p = PropertyChanged;
                if (p != null)
                    p(this, new PropertyChangedEventArgs(null));

                if (_info.IsEN)
                    _generatedResource = new Generated_en();
                if (_info.IsDE)
                    _generatedResource = new Generated_de();
                if (_info.IsRU)
                    _generatedResource = new Generated_ru();
                if (_info.IsFR)
                    _generatedResource = new Generated_fr();
                if (_info.IsIT)
                    _generatedResource = new Generated_it();

            };
        }

        IGeneratedResource _generatedResource = new Generated_en();

       


    }
}
