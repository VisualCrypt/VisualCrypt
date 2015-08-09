using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace VisualCrypt.Windows.Static
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
		public int SerializableFontStretch { get; set;
		    //get { return FontStretch.ToOpenTypeStretch(); }
		    //set { FontStretch = FontStretch.FromOpenTypeStretch(value); }
		}

		[DataMember]
		public string SerializableFontStyle
		{
			get { return FontStyle.ToString(); }
			set { FontStyle = GetFontStyleFromString(value); }
		}

		[DataMember]
		public int SerializableFontWeight { get; set;
		    //get { return FontWeight.ToOpenTypeWeight(); }
		    //set { FontWeight = FontWeight.FromOpenTypeWeight(value); }
		}


		FontStyle GetFontStyleFromString(string fonstStyleString)
		{
			if (string.IsNullOrWhiteSpace(fonstStyleString))
				throw new ArgumentException("Invalid FontStyle descriptor.");

			switch (fonstStyleString)
			{
				case "Normal":
                    break;
					//return FontStyles.Normal;
				case "Oblique":
                    break;
                    //return FontStyles.Oblique;
				case "Italic":
                    break;
                    //return FontStyles.Italic;
				default:
					throw new ArgumentException("Invalid FontStyle descriptor.");
			}

            // todo: fix
            return FontStyle.Normal;
		}


		public void ApplyTo(TextBox textBox1)
		{
			if (textBox1 == null)
				throw new ArgumentNullException("textBox1");
			try
			{
				//if (FontFamily == null)
				//	FontFamily = SystemFonts.MessageFontFamily;
				//if (FontSize < 5)
				//	FontSize = SystemFonts.MessageFontSize;

				Map.Copy(this, textBox1);
			}
			catch (Exception e)
			{
                Debug.WriteLine(e.Message);
			}
		}
	}
}