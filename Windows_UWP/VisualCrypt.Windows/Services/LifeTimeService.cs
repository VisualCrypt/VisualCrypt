using System;
using System.ComponentModel;
using System.Threading.Tasks;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Windows.Services
{
    public class LifeTimeService : ILifeTimeService
    {
        public void HandleExitRequested(CancelEventArgs e, Func<Task<bool>> confirmDiscard)
        {
            throw new NotImplementedException();
        }

      
    }
}