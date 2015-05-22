using System.Runtime.Serialization;

namespace VisualCrypt.Desktop.State
{
    /// <summary>
    /// Tracks the persited state of the editor.
    /// </summary>
    [DataContract]
    public class EditorState
    {
       
        [DataMember]
        public bool IsStatusBarChecked { get; set; }

        [DataMember]
        public bool IsWordWrapChecked { get; set; }

        [DataMember]
        public bool IsSpellCheckingChecked { get; set; }
    }
}