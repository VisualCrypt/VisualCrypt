using Android.App;
using Android.Content;

namespace VisualCrypt.Droid
{
    class VisualCryptApplication : Application
    {
        static Application _app;

        public override void OnCreate()
        {
            base.OnCreate();
            _app = this;
        }

        public static Context GetAppContext()
        {
            return _app.ApplicationContext;
        }

    }
}