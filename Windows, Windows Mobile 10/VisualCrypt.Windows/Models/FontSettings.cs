using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using VisualCrypt.Applications.Models.Settings;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace VisualCrypt.Windows.Models
{
    [DataContract]
    public class FontSettings : IFontSettings, INotifyPropertyChanged
    {

        public FontFamily FontFamily { get; set; }


        public FontWeight FontWeight { get; set; }

        [DataMember]
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
        public ushort SerializableFontWeight
        {
            get { return FontWeight.Weight; }
            set { FontWeight = new FontWeight { Weight = value }; }
        }





        public void ApplyToTextBox(TextBox textBox)
        {

            if (FontFamily == null)
                FontFamily = new FontFamily("Lucida Console");
            if (FontSize < 5)
                FontSize = 5;

            textBox.FontFamily = FontFamily;
            textBox.FontWeight = FontWeight;
            textBox.FontStyle = FontStyle;
            textBox.FontStretch = FontStretch;
            textBox.FontSize = FontSize;

        }

        internal void ApplyToFlowDocument(RichTextBlock flowDocument)
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