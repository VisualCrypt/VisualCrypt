using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VisualCrypt.Cryptography.Portable.Annotations;

namespace VisualCrypt.Cryptography.Portable
{
	public static class Loc
	{
		

		public static Strings Strings
		{
			get
			{
				if(_strings == null)
				{ 
					_strings = new Strings();
				
				}
				return _strings;
			}
		}

		static Strings _strings;

		public static void SetLanguage(string loc)
		{
			_strings.SwitchDictionary(loc);
		}
	}

	

	public class Strings : INotifyPropertyChanged
	{
		Dictionary<string, string> CurrentDictionary;
		string _currentLocale;

		public Strings()
		{
			CurrentDictionary = CreateEnglishDictionary();
			_currentLocale = "EN";
		}

		Dictionary<string, string> CreateEnglishDictionary()
		{
			var dict = new Dictionary<string, string>();
			dict.Add("miLanguage", "Language");
			return dict;
		}

		Dictionary<string, string> CreateGermanDictionary()
		{
			var dict = new Dictionary<string, string>();
			dict.Add("miLanguage", "Sprache");
			return dict;
		}


		public string miLanguage
		{
			get { return CurrentDictionary["miLanguage"]; }
		}

		public bool IsEN
		{
			get { return _currentLocale == "EN"; }
		}

		public bool IsDE
		{
			get { return _currentLocale == "DE"; }
		}

		

		public void RaiseChanged()
		{
			OnPropertyChanged("miLanguage");
			OnPropertyChanged("IsDE");
			OnPropertyChanged("IsEN");
		}

		private static event PropertyChangedEventHandler _propertyChanged;
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = _propertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}


		internal void SwitchDictionary(string locale)
		{
			
			switch (locale)
			{
				case "EN":
					CurrentDictionary = CreateEnglishDictionary();
					break;
				case "DE":
					CurrentDictionary = CreateGermanDictionary();
					break;
			}
			_currentLocale = locale;
			RaiseChanged();
			
		}
	}

	

	
}
