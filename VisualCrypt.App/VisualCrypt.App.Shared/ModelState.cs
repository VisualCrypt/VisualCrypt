namespace VisualCrypt.App
{
    public static class ModelState
    {
        public static string Password
        {
            get { return _password ?? string.Empty; }
            set { _password = value; }
        }
        static string _password;

        public static string TextBuffer
        {
            get { return _textBuffer ?? string.Empty; }
            set { _textBuffer = value; }
        }
        static string _textBuffer;

        public static string FileName
        {
            get { return _filename ?? string.Empty; }
            set { _filename = value; }
        }
        static string _filename;

       
    }
}
