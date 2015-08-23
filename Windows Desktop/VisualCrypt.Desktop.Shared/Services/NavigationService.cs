using System;
using System.ComponentModel.Composition;
using VisualCrypt.Applications.Apps.Models;
using VisualCrypt.Applications.Apps.Services;

namespace VisualCrypt.Desktop.Shared.Services
{
	[Export(typeof(INavigationService))]
	class NavigationService : INavigationService
	{
		public void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs)
		{
			throw new NotImplementedException();
		}

		public void NavigateToFilesPage()
		{
			throw new NotImplementedException();
		}
	}
}
