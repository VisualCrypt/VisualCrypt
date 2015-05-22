using System.Runtime.Serialization;
using VisualCrypt.Desktop.Models.Fonts;
using VisualCrypt.Desktop.Models.Printing;

namespace VisualCrypt.Desktop.State
{
    [DataContract]
    public class NotepadSettings
    {
        [DataMember]
        public FontSettings FontSettings { get; set; }

        [DataMember]
        public PageSettings PageSettings { get; set; }

        [DataMember]
        public EditorState EditorState { get; set; }
      
    }
}
