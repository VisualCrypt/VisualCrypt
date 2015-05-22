namespace VisualCrypt.Portable.APIV2.DataTypes
{
    public enum ContentKind : byte
    {
        Undefined = 0,
        PlainText = 1,
        Binary = 2,
        EncryptedText = 3,
        EncryptedBinary = 4
    }
}
