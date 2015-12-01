using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Views;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.Droid.Services
{
    class NavigationService : INavigationService
    {
        public void NavigateToFilesPage()
        {
            throw new NotImplementedException();
        }

        public void NavigateToHelpPage()
        {
            throw new NotImplementedException();
        }

        public void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs)
        {
            FilesActivity.Instance.StartActivity(typeof(MainActivity));
            Applications.Services.Interfaces.Service.Get<PortableMainViewModel>().OnNavigatedToCompletedAndLoaded(filesPageCommandArgs);

        }

        public void NavigateToSettingsPage()
        {
            throw new NotImplementedException();
        }
    }
}