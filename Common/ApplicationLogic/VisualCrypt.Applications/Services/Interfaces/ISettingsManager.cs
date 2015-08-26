using VisualCrypt.Applications.Models.Settings;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface ISettingsManager
    {
        string CurrentDirectoryName { get; set; }
        EditorSettings EditorSettings { get; }

        CryptographySettings CryptographySettings { get; }

        object FontSettings { get; }
	    void LoadOrInitSettings();
    }
}
