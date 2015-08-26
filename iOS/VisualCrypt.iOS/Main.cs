using System;
using UIKit;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;

namespace VisualCrypt.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            var api = Service.Get<IVisualCrypt2Service>();
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}