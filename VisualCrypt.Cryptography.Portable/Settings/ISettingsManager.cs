namespace VisualCrypt.Cryptography.Portable.Settings
{
    public interface ISettingsManager
    {
        string CurrentDirectoryName { get; set; }
        EditorSettings EditorSettings { get; }

        CryptographySettings CryptographySettings { get; }

        object FontSettings { get; }
    }
}
