using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Android.App;
using VisualCrypt.Applications.Services.Interfaces;


namespace VisualCrypt.Droid
{
    class MessageBoxService : IMessageBoxService
    {
        public Task<RequestResult> Show(string messageBoxText, string title, RequestButton buttons, RequestImage image)
        {

           
            var builder = new AlertDialog.Builder(null);

            builder.SetTitle("Hello Dialog")
                   .SetMessage("Is this material design?")
                   .SetPositiveButton("Yes", delegate { Console.WriteLine("Yes"); })
                   .SetNegativeButton("No", delegate { Console.WriteLine("No"); });

            builder.Create().Show();
            return null;

        }

        public Task ShowError(string error)
        {
            throw new NotImplementedException();
        }

        public Task ShowError(Exception e, [CallerMemberName] string callerMemberName = "")
        {
            throw new NotImplementedException();
        }
    }
}