namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
    public class Response
    {
        public bool Success = false;
        public string Error = string.Empty;
    }

    public sealed class Response<T> : Response
    {
        public T Result;
    }
}
