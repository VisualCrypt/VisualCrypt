using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace VisualCrypt.Desktop.Settings
{
    [DataContract]
    public class FontSettings : INotifyPropertyChanged
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


        public void ApplyToTextBox(TextBox textBox)
        {

            if (FontFamily == null)
                FontFamily = SystemFonts.MessageFontFamily;
            if (FontSize < 5)
                FontSize = SystemFonts.MessageFontSize;

            textBox.FontFamily = FontFamily;
            textBox.FontWeight = FontWeight;
            textBox.FontStyle = FontStyle;
            textBox.FontStretch = FontStretch;
            textBox.FontSize = FontSize;

        }

        internal void ApplyToFlowDocument(FlowDocument flowDocument)
        {
            flowDocument.FontFamily = FontFamily;
            flowDocument.FontWeight = FontWeight;
            flowDocument.FontStyle = FontStyle;
            flowDocument.FontStretch = FontStretch;
            flowDocument.FontSize = FontSize;

        }

        public FontSettings Clone()
        {
            return new FontSettings
            {
                FontFamily = FontFamily,
                FontWeight = FontWeight,
                FontStyle = FontStyle,
                FontStretch = FontStretch,
                FontSize = FontSize
            };
        }

        public void UpdateFrom(FontSettings other)
        {
            FontFamily = other.FontFamily;
            FontWeight = other.FontWeight;
            FontStyle = other.FontStyle;
            FontStretch = other.FontStretch;
            FontSize = other.FontSize;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}