using System.Text;
using VisualCrypt.Desktop.Shared.App;

namespace VisualCrypt.Desktop.Shared.Files
{
    public class EmptyCleartextFileModel : FileModelBase
    {
        public EmptyCleartextFileModel()
        {
            IsDirty = false;
            Filename = Constants.UntitledDotVisualCrypt;
            Contents = string.Empty;
            SaveEncoding = Encoding.UTF8;
        }
    }
}
