using System.Runtime.Serialization;

namespace VisualCrypt.Desktop.Models
{
    [DataContract]
    public class SerializableVisualCryptSettings
    {
        [DataMember]
        public Applications.Models.Settings.EditorSettings EditorSettings { get; set; }

        [DataMember]
        public Applications.Models.Settings.CryptographySettings CryptographySettings { get; set; }

        [DataMember]
        public Settings.FontSettings FontSettings { get; set; }
    }
}
