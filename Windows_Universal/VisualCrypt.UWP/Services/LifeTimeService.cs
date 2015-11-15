using System;
using System.ComponentModel;
using System.Threading.Tasks;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.UWP.Services
{
    public class LifeTimeService : ILifeTimeService
    {
        public void HandleExitRequested(CancelEventArgs e, Func<Task<bool>> confirmDiscard)
        {
            if (e == null && confirmDiscard == null)
            {
                Windows.UI.Xaml.Application.Current.Exit();
            }
            else
            {
                throw new Exception("This code should not run in the UWP App!");
                //var canExit = await confirmDiscard();
                //if (!canExit)
                //    e.Cancel = true;
            }
          
        }
    }
}