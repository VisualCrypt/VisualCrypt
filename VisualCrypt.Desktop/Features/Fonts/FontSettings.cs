using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace VisualCrypt.Desktop.Models.Fonts
{
    [DataContract]
    public class FontSettings
    {

        public FontFamily FontFamily { get; set; }

       
        public FontWeight FontWeight { get; set; }

        public FontStyle FontStyle { get; set; }

        [DataMember]
        public FontStretch FontStretch { get; set; }

        [DataMember]
        public double FontSize { get; set; }

        [DataMember]
        public string SerializableFontFamily
        {
            get { return FontFamily.Source; }
            set { FontFamily = new FontFamily(value); }
        }

        [DataMember]
        public int SerializableFontStretch
        {
            get { return FontStretch.ToOpenTypeStretch(); }
            set { FontStretch = FontStretch.FromOpenTypeStretch(value); }
        }

        [DataMember]
        public string SerializableFontStyle
        {
            get { return FontStyle.ToString(); }
            set { FontStyle = GetFontStyleFromString(value); }
        }

        [DataMember]
        public int SerializableFontWeight
        {
            get { return FontWeight.ToOpenTypeWeight(); }
            set { FontWeight = FontWeight.FromOpenTypeWeight(value); }
        }


        FontStyle GetFontStyleFromString(string fonstStyleString)
        {
            if (string.IsNullOrWhiteSpace(fonstStyleString))
                throw new ArgumentException("Invalid FontStyle descriptor.");

            switch (fonstStyleString)
            {
                case "Normal":
                    return FontStyles.Normal;
                case "Oblique":
                    return FontStyles.Oblique;
                case "Italic":
                    return FontStyles.Italic;
                default:
                    throw new ArgumentException("Invalid FontStyle descriptor.");
            }

        }


    }
}