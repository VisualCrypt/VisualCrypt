using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Droid.Services
{
    class LifeTimeService : ILifeTimeService
    {
        public void HandleExitRequested(CancelEventArgs e, Func<Task<bool>> confirmDiscard)
        {
            throw new NotImplementedException();
        }
    }
}