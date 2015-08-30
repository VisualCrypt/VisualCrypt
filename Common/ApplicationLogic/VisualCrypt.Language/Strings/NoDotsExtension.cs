namespace VisualCrypt.Language.Strings
{
    public static class NoDotsExtension
    {
        /// <summary>
        /// Allows the reuse of menu item texts such as 'Replace...' for button texts such as 'Replace'.
        /// </summary>
        public static string NoDots(this string withDots)
        {
            return withDots.Replace(".", string.Empty);
        }
    }
}
