using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Cryptography.Portable.Apps.Services;

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
