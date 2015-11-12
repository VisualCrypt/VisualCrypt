﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Windows.Pages;
using System;

namespace VisualCrypt.Windows.Services
{
    class NavigationService : INavigationService
    {

        public void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs)
        {
            var frame = Window.Current.Content as Frame;
            if (App.IsPhoneLayout())
                frame.Navigate(typeof(MainPagePhone), filesPageCommandArgs, new DrillInNavigationTransitionInfo());
            else
                throw new System.Exception();
                //frame.Navigate(typeof(MainPage), filesPageCommandArgs, new DrillInNavigationTransitionInfo());
        }

        public void NavigateToFilesPage()
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(FilesPage), new DrillInNavigationTransitionInfo());
        }

        public void NavigateToHelpPage()
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(HelpPage));
        }

        public void NavigateToSettingsPage()
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(SettingsPage));
        }
    }
}