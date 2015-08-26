using System;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Desktop.Services
{
	class NavigationService : INavigationService
	{
	    object _context;

	    public object Context
	    {
	        set { _context = value; }
	    }

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
