using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VisualCrypt.Desktop.ModuleEditor.FeatureSupport.Fonts;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.PrismSupport;
using VisualCrypt.Desktop.Shared.Settings;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
	/// <summary>
	///     Font Chooser adapted from Niklas Borson's code sample:
	///     http://blogs.msdn.com/b/text/archive/2006/11/01/sample-font-chooser.aspx
	///     [...] The attached sample code is provided as is. I won't promise it's perfect, but I hope it helps you get
	///     started. Enjoy!
	///     --Niklas Borson,  FontDialogSample.zip
	/// </summary>
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class Font
	{
		#region fields and types

		const string DefaultSampleText = "The quick brown fox jumps over the lazy dog";
		readonly string _sampleText;
		readonly FontSettings _selectedFontSettings;

		ICollection<FontFamily> _familyCollection; // see FamilyCollection property

		bool _updatePending; // indicates a call to OnUpdate is scheduled
		bool _familyListValid; // indicates the list of font families is valid
		bool _typefaceListValid; // indicates the list of typefaces is valid
		bool _typefaceListSelectionValid; // indicates the current selection in the typeface list is valid
		bool _previewValid; // indicates the preview control is valid


		static readonly double[] CommonlyUsedFontSizes =
		{
			3.0, 4.0, 5.0, 6.0, 6.5,
			7.0, 7.5, 8.0, 8.5, 9.0,
			9.5, 10.0, 10.5, 11.0, 11.5,
			12.0, 12.5, 13.0, 13.5, 14.0,
			15.0, 16.0, 17.0, 18.0, 19.0,
			20.0, 22.0, 24.0, 26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0,
			40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0,
			80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
		};

		#endregion

		#region Construction and initialization

		[ImportingConstructor]
		public Font(IParamsProvider paramsProvider)
		{
			string sampleText = paramsProvider.GetParams<Font, string>();
			_selectedFontSettings = new FontSettings();
			Map.Copy(SettingsManager.EditorSettings.FontSettings, _selectedFontSettings);

			_sampleText = string.IsNullOrWhiteSpace(sampleText)
				? DefaultSampleText
				: sampleText;

			InitializeComponent();

			PreviewKeyDown += CloseWithEscape;
		}

		void CloseWithEscape(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			PreviewTextBox.Text = _sampleText;

			// Hook up events for the font family list and associated text box.
			FontFamilyTextBox.SelectionChanged += fontFamilyTextBox_SelectionChanged;
			FontFamilyTextBox.TextChanged += fontFamilyTextBox_TextChanged;
			FontFamilyTextBox.PreviewKeyDown += fontFamilyTextBox_PreviewKeyDown;
			FontFamilyList.SelectionChanged += fontFamilyList_SelectionChanged;

			// Hook up events for the typeface list.
			TypefaceList.SelectionChanged += typefaceList_SelectionChanged;

			// Hook up events for the font size list and associated text box.
			SizeTextBox.TextChanged += sizeTextBox_TextChanged;
			SizeTextBox.PreviewKeyDown += sizeTextBox_PreviewKeyDown;
			SizeList.SelectionChanged += sizeList_SelectionChanged;

			// Initialize the list of font sizes and select the nearest size.
			foreach (double value in CommonlyUsedFontSizes)
			{
				SizeList.Items.Add(new FontSizeListItem(value));
			}
			OnSelectedFontSizeChanged(_selectedFontSettings.FontSize);

			// Initialize the font family list and the current family.
			if (!_familyListValid)
			{
				InitializeFontFamilyList();
				_familyListValid = true;
				OnSelectedFontFamilyChanged(_selectedFontSettings.FontFamily);
			}

			// Schedule background updates.
			ScheduleUpdate();
			InitializePreview();
		}

		#endregion

		#region Event handlers

		void OnOkButtonClicked(object sender, RoutedEventArgs e)
		{
			Map.Copy(_selectedFontSettings, SettingsManager.EditorSettings.FontSettings);

			DialogResult = true;
			Close();
		}

		void OnCancelButtonClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}

		int _fontFamilyTextBoxSelectionStart;

		void fontFamilyTextBox_SelectionChanged(object sender, RoutedEventArgs e)
		{
			_fontFamilyTextBoxSelectionStart = FontFamilyTextBox.SelectionStart;
		}

		void fontFamilyTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string text = FontFamilyTextBox.Text;

			// Update the current list item.
			if (SelectFontFamilyListItem(text) == null)
			{
				// The text does not exactly match a family name so consider applying auto-complete behavior.
				// However, only do so if the following conditions are met:
				//   (1)  The user is typing more text rather than deleting (i.e., the new text length is
				//        greater than the most recent selection start index), and
				//   (2)  The caret is at the end of the text box.
				if (text.Length > _fontFamilyTextBoxSelectionStart
				    && FontFamilyTextBox.SelectionStart == text.Length)
				{
					// Get the current list item, which should be the nearest match for the text.
					FontFamilyListItem item = FontFamilyList.Items.CurrentItem as FontFamilyListItem;
					if (item != null)
					{
						// Does the text box text match the beginning of the family name?
						string familyDisplayName = item.ToString();
						if (
							string.Compare(text, 0, familyDisplayName, 0, text.Length, true, CultureInfo.CurrentCulture) ==
							0)
						{
							// Set the text box text to the complete family name and select the part not typed in.
							FontFamilyTextBox.Text = familyDisplayName;
							FontFamilyTextBox.SelectionStart = text.Length;
							FontFamilyTextBox.SelectionLength = familyDisplayName.Length - text.Length;
						}
					}
				}
			}
		}

		void sizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			double sizeInPoints;
			if (double.TryParse(SizeTextBox.Text, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture,
				out sizeInPoints))
			{
				double sizeInPixels = FontSizeListItem.PointsToPixels(sizeInPoints);
				if (!FontSizeListItem.FuzzyEqual(sizeInPixels, _selectedFontSettings.FontSize))
				{
					_selectedFontSettings.FontSize = sizeInPixels;
				}
			}
		}

		void fontFamilyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			OnComboBoxPreviewKeyDown(FontFamilyList, e);
		}

		void sizeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			OnComboBoxPreviewKeyDown(SizeList, e);
		}

		void fontFamilyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FontFamilyListItem item = FontFamilyList.SelectedItem as FontFamilyListItem;
			if (item != null)
			{
				_selectedFontSettings.FontFamily = item.FontFamily;
				PreviewTextBox.FontFamily = item.FontFamily;
				InvalidateTypefaceList();
			}
		}

		void sizeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FontSizeListItem item = SizeList.SelectedItem as FontSizeListItem;
			if (item != null)
			{
				_selectedFontSettings.FontSize = item.SizeInPixels;
				PreviewTextBox.FontSize = item.SizeInPixels;
			}
		}

		void typefaceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TypefaceListItem item = TypefaceList.SelectedItem as TypefaceListItem;
			if (item != null)
			{
				_selectedFontSettings.FontWeight = item.FontWeight;
				_selectedFontSettings.FontStyle = item.FontStyle;
				_selectedFontSettings.FontStretch = item.FontStretch;

				PreviewTextBox.FontWeight = item.FontWeight;
				PreviewTextBox.FontStyle = item.FontStyle;
				PreviewTextBox.FontStretch = item.FontStretch;
				InvalidateTypefaceListSelection();
			}
		}

		#endregion

		#region Public properties and methods

		/// <summary>
		/// Collection of font families to display in the font family list. By default this is Fonts.SystemFontFamilies,
		/// but a client could set this to another collection returned by Fonts.GetFontFamilies, e.g., a collection of
		/// application-defined fonts.
		/// </summary>
		public ICollection<FontFamily> FontFamilyCollection
		{
			get { return _familyCollection ?? System.Windows.Media.Fonts.SystemFontFamilies; }

			set
			{
				if (Equals(value, _familyCollection))
					return;
				_familyCollection = value;
				InvalidateFontFamilyList();
			}
		}

		/// <summary>
		/// Sets the font chooser selection properties to match the properites of the specified object.
		/// </summary>
		//public void SetPropertiesFromObject(DependencyObject obj)
		//{
		//    foreach (DependencyProperty property in _chooserProperties)
		//    {
		//        FontPropertyMetadata metadata = property.GetMetadata(typeof(Font)) as FontPropertyMetadata;
		//        if (metadata != null)
		//        {
		//            this.SetValue(property, obj.GetValue(metadata.TargetProperty));
		//        }
		//    }
		//}
		/// <summary>
		/// Sets the properites of the specified object to match the font chooser selection properties.
		/// </summary>
		//public void ApplyPropertiesToObject(DependencyObject obj)
		//{
		//    foreach (DependencyProperty property in _chooserProperties)
		//    {
		//        FontPropertyMetadata metadata = property.GetMetadata(typeof(Font)) as FontPropertyMetadata;
		//        if (metadata != null)
		//        {
		//            obj.SetValue(metadata.TargetProperty, this.GetValue(property));
		//        }
		//    }
		//}

		#endregion

		#region Property change handlers

		// Handle changes to the SelectedFontFamily property
		void OnSelectedFontFamilyChanged(FontFamily family)
		{
			// If the family list is not valid do nothing for now. 
			// We'll be called again after the list is initialized.
			if (_familyListValid)
			{
				// Select the family in the list; this will return null if the family is not in the list.
				FontFamilyListItem item = SelectFontFamilyListItem(family);

				// Set the text box to the family name, if it isn't already.
				string displayName = (item != null) ? item.ToString() : FontFamilyListItem.GetDisplayName(family);
				if (string.Compare(FontFamilyTextBox.Text, displayName, true, CultureInfo.CurrentCulture) != 0)
				{
					FontFamilyTextBox.Text = displayName;
				}

				// The typeface list is no longer valid; update it in the background to improve responsiveness.
				InvalidateTypefaceList();
			}
		}

		// Handle changes to the SelectedFontSize property
		void OnSelectedFontSizeChanged(double sizeInPixels)
		{
			// Select the list item, if the size is in the list.
			double sizeInPoints = FontSizeListItem.PixelsToPoints(sizeInPixels);
			if (!SelectListItem(SizeList, sizeInPoints))
			{
				SizeList.SelectedIndex = -1;
			}

			// Set the text box contents if it doesn't already match the current size.
			double textBoxValue;
			if (
				!double.TryParse(SizeTextBox.Text, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture,
					out textBoxValue) || !FontSizeListItem.FuzzyEqual(textBoxValue, sizeInPoints))
			{
				SizeTextBox.Text = sizeInPoints.ToString(CultureInfo.CurrentCulture);
			}

			// Schedule background updates.

			InvalidatePreview();
		}

		#endregion

		#region Background update logic

		// Schedule background initialization of the font famiy list.
		void InvalidateFontFamilyList()
		{
			if (_familyListValid)
			{
				InvalidateTypefaceList();

				FontFamilyList.Items.Clear();
				FontFamilyTextBox.Clear();
				_familyListValid = false;

				ScheduleUpdate();
			}
		}

		// Schedule background initialization of the typeface list.
		void InvalidateTypefaceList()
		{
			if (_typefaceListValid)
			{
				TypefaceList.Items.Clear();
				_typefaceListValid = false;

				ScheduleUpdate();
			}
		}

		// Schedule background selection of the current typeface list item.
		void InvalidateTypefaceListSelection()
		{
			if (_typefaceListSelectionValid)
			{
				_typefaceListSelectionValid = false;
				ScheduleUpdate();
			}
		}


		// Schedule background initialization of the preview control.
		void InvalidatePreview()
		{
			if (_previewValid)
			{
				_previewValid = false;
				ScheduleUpdate();
			}
		}

		// Schedule background initialization.
		void ScheduleUpdate()
		{
			if (!_updatePending)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(OnUpdate));
				_updatePending = true;
			}
		}

		// Dispatcher callback that performs background initialization.
		void OnUpdate()
		{
			_updatePending = false;

			if (!_familyListValid)
			{
				// Initialize the font family list.
				InitializeFontFamilyList();
				_familyListValid = true;
				OnSelectedFontFamilyChanged(_selectedFontSettings.FontFamily);

				// Defer any other initialization until later.
				ScheduleUpdate();
			}
			else if (!_typefaceListValid)
			{
				// Initialize the typeface list.
				InitializeTypefaceList();
				_typefaceListValid = true;

				// Select the current typeface in the list.
				InitializeTypefaceListSelection();
				_typefaceListSelectionValid = true;

				// Defer any other initialization until later.
				ScheduleUpdate();
			}
			else if (!_typefaceListSelectionValid)
			{
				// Select the current typeface in the list.
				InitializeTypefaceListSelection();
				_typefaceListSelectionValid = true;

				// Defer any other initialization until later.
				ScheduleUpdate();
			}
			else
			{
				// Perform any remaining initialization.

				if (!_previewValid)
				{
					// Initialize the preview control.
					InitializePreview();
					_previewValid = true;
				}
			}
		}

		#endregion

		#region Content initialization

		void InitializeFontFamilyList()
		{
			ICollection<FontFamily> familyCollection = FontFamilyCollection;
			if (familyCollection != null)
			{
				var items = new FontFamilyListItem[familyCollection.Count];

				var i = 0;

				foreach (FontFamily family in familyCollection)
				{
					items[i++] = new FontFamilyListItem(family);
				}

				Array.Sort(items);

				foreach (var item in items)
				{
					FontFamilyList.Items.Add(item);
				}
			}
		}

		void InitializeTypefaceList()
		{
			FontFamily family = _selectedFontSettings.FontFamily;
			if (family != null)
			{
				ICollection<Typeface> faceCollection = family.GetTypefaces();

				TypefaceListItem[] items = new TypefaceListItem[faceCollection.Count];

				int i = 0;

				foreach (Typeface face in faceCollection)
				{
					items[i++] = new TypefaceListItem(face);
				}

				Array.Sort(items);

				foreach (TypefaceListItem item in items)
				{
					TypefaceList.Items.Add(item);
				}
			}
		}

		void InitializeTypefaceListSelection()
		{
			// If the typeface list is not valid, do nothing for now.
			// We'll be called again after the list is initialized.
			if (_typefaceListValid)
			{
				Typeface typeface = new Typeface(_selectedFontSettings.FontFamily, _selectedFontSettings.FontStyle,
					_selectedFontSettings.FontWeight, _selectedFontSettings.FontStretch);

				// Select the typeface in the list.
				SelectTypefaceListItem(typeface);

				// Schedule background updates.

				InvalidatePreview();
			}
		}


		void InitializePreview()
		{
			Map.Copy(_selectedFontSettings, PreviewTextBox);
		}

		#endregion

		#region List box helpers

		// Update font family list based on selection.
		// Return list item if there's an exact match, or null if not.
		FontFamilyListItem SelectFontFamilyListItem(string displayName)
		{
			var listItem = FontFamilyList.SelectedItem as FontFamilyListItem;

			if (listItem != null &&
			    string.Compare(listItem.ToString(), displayName, true, CultureInfo.CurrentCulture) == 0)
			{
				// Already selected
				return listItem;
			}
			if (SelectListItem(FontFamilyList, displayName))
			{
				// Exact match found
				return FontFamilyList.SelectedItem as FontFamilyListItem;
			}
			// Not in the list
			return null;
		}

		// Update font family list based on selection.
		// Return list item if there's an exact match, or null if not.
		FontFamilyListItem SelectFontFamilyListItem(FontFamily family)
		{
			FontFamilyListItem listItem = FontFamilyList.SelectedItem as FontFamilyListItem;
			if (listItem != null && listItem.FontFamily.Equals(family))
			{
				// Already selected
				return listItem;
			}
			if (SelectListItem(FontFamilyList, FontFamilyListItem.GetDisplayName(family)))
			{
				// Exact match found
				return FontFamilyList.SelectedItem as FontFamilyListItem;
			}
			// Not in the list
			return null;
		}

		// Update typeface list based on selection.
		// Return list item if there's an exact match, or null if not.
		void SelectTypefaceListItem(Typeface typeface)
		{
			var listItem = TypefaceList.SelectedItem as TypefaceListItem;

			// already selected
			if (listItem != null && listItem.Typeface.Equals(typeface))
				return;

			SelectListItem(TypefaceList, new TypefaceListItem(typeface));
		}

		// Update list based on selection.
		// Return true if there's an exact match, or false if not.
		bool SelectListItem(ListBox list, object value)
		{
			ItemCollection itemList = list.Items;

			// Perform a binary search for the item.
			int first = 0;
			int limit = itemList.Count;

			while (first < limit)
			{
				int i = first + (limit - first)/2;
				IComparable item = (IComparable) (itemList[i]);
				int comparison = item.CompareTo(value);
				if (comparison < 0)
				{
					// DataBytes must be after i
					first = i + 1;
				}
				else if (comparison > 0)
				{
					// DataBytes must be before i
					limit = i;
				}
				else
				{
					// Exact match; select the item.
					list.SelectedIndex = i;
					itemList.MoveCurrentToPosition(i);
					list.ScrollIntoView(itemList[i]);
					return true;
				}
			}

			// Not an exact match; move current position to the nearest item but don't select it.
			if (itemList.Count > 0)
			{
				int i = Math.Min(first, itemList.Count - 1);
				itemList.MoveCurrentToPosition(i);
				list.ScrollIntoView(itemList[i]);
			}

			return false;
		}

		// Logic to handle UP and DOWN arrow keys in the text box associated with a list.
		// Behavior is similar to a Win32 combo box.
		void OnComboBoxPreviewKeyDown(ListBox listBox, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Up:
					// Move up from the current position.
					MoveListPosition(listBox, -1);
					e.Handled = true;
					break;

				case Key.Down:
					// Move down from the current position, unless the item at the current position is
					// not already selected in which case select it.
					if (listBox.Items.CurrentPosition == listBox.SelectedIndex)
					{
						MoveListPosition(listBox, +1);
					}
					else
					{
						MoveListPosition(listBox, 0);
					}
					e.Handled = true;
					break;
			}
		}

		void MoveListPosition(ListBox listBox, int distance)
		{
			int i = listBox.Items.CurrentPosition + distance;
			if (i >= 0 && i < listBox.Items.Count)
			{
				listBox.Items.MoveCurrentToPosition(i);
				listBox.SelectedIndex = i;
				listBox.ScrollIntoView(listBox.Items[i]);
			}
		}

		#endregion
	}
}