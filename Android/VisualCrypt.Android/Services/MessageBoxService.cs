using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VisualCrypt.Applications.Services.Interfaces;
using AlertDialog = Android.Support.V7.App.AlertDialog;


namespace VisualCrypt.Droid
{
    class MessageBoxService : IMessageBoxService
    {
        public Task<RequestResult> Show(string messageBoxText, string title, RequestButton buttons, RequestImage image)
        {

           
            var builder = new AlertDialog.Builder(VisualCryptApplication.GetAppContext());

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