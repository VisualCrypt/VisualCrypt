// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Windows;

namespace VisualCrypt.Desktop
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// The boostrapper will create the Shell instance, so the App.xaml does not have a StartupUri.
			ShellBootstrapper bootstrapper = new ShellBootstrapper();
			bootstrapper.Run();
		}
	}
}