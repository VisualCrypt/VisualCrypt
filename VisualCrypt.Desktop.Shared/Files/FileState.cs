namespace VisualCrypt.Desktop.Shared.Files
{
    public static class FileState
    {
        public static FileModel FileModel { get; set; }
        static FileState ()
        {
            FileModel = new FileModel();
        }
    }
}
