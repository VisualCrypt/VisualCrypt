using System;
using UIKit;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.Net;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;

namespace VisualCrypt.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            Register();
          
            UIApplication.Main(args, null, "AppDelegate");
        }

        static void Register()
        {
            // Bootstrap the VisualCrypt API
            Service.Register<IPlatform, Platform_Net4>(true);
            Service.Register<IVisualCrypt2Service, VisualCrypt2Service>(true);
            Service.Get<IVisualCrypt2Service>().Platform = Service.Get<IPlatform>();
        }
    }
}