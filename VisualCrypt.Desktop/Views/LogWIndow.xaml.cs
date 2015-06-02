// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using VisualCrypt.Desktop.Shared;

namespace VisualCrypt.Desktop.Views
{
	/// <summary>
	/// Interaction logic for LogWindow.xaml
	/// </summary>
	[Export]
	public partial class LogWindow : IPartImportsSatisfiedNotification
	{
#pragma warning disable 0649 // Imported by MEF

		// The shell imports IModuleManager once to load modules on-demand.
		[Import(AllowRecomposition = false)] IModuleManager moduleManager;

		// The shell imports the logger once to output logs to the UI.
		[Import(AllowRecomposition = false)] CallbackLogger logger;
#pragma warning restore 0649 

		/// <summary>
		/// Initializes a new instance of the <see cref="LogWindow"/> class.
		/// </summary>
		public LogWindow()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Logs the specified message.  Called by the CallbackLogger.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="category">The category.</param>
		/// <param name="priority">The priority.</param>
		public void Log(string message, Category category, Priority priority)
		{
			this.TraceTextBox.AppendText(
				string.Format(
					CultureInfo.CurrentUICulture,
					"[{0}][{1}] {2}\r\n",
					category,
					priority,
					message));
		}

		/// <summary>
		/// Called when a part's imports have been satisfied and it is safe to use.
		/// </summary>
		public void OnImportsSatisfied()
		{
			// IPartImportsSatisfiedNotification is useful when you want to coordinate doing some work
			// with imported parts independent of when the UI is visible.

			// I use the IModuleTracker as the data-context for data-binding.
			// This quickstart only demonstrates modularity for Prism and does not use data-binding patterns such as MVVM.


			// I set this shell's Log method as the callback for receiving log messages.
			this.logger.Callback = this.Log;
			this.logger.ReplaySavedLogs();
		}
	}
}