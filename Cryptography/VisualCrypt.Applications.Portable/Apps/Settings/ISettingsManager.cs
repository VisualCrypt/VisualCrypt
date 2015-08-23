namespace VisualCrypt.Applications.Portable.Apps.Settings
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
