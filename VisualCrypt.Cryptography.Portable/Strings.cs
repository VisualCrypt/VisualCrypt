using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using VisualCrypt.Cryptography.Portable.Annotations;

namespace VisualCrypt.Cryptography.Portable
{
	public class Strings : INotifyPropertyChanged
	{
		#region Localization engine

		readonly List<string> _propertyNames;

		public Strings()
		{
			Localization.SetLanguageOrder(new[] { "EN", "DE" });
			Localization.SetCurrentLocale("EN");

			_propertyNames = new List<string>();
			var properties = GetType().GetRuntimeProperties();
			foreach (var propertyInfo in properties)
			{
				_propertyNames.Add(propertyInfo.Name);
			}
		}

		internal void SwitchLocale(string locale)
		{
			Localization.SetCurrentLocale(locale);
			foreach (var prop in _propertyNames)
			{
				OnPropertyChanged(prop);
			}
		}

		public bool IsEN
		{
			get { return Localization.GetCurrentLocale() == "EN"; }
		}

		public bool IsDE
		{
			get { return Localization.GetCurrentLocale() == "DE"; }
		}

		#endregion

		#region Languages

		// MenuItems

		public string miLanguage
		{
			get
			{
				return new[] { "_Language", "_Sprache" }.GetLocalizedString();
			}
		}

		public string miFile
		{
			get
			{
				return new[] { "_File", "_Datei" }.GetLocalizedString();
			}
		}











		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
