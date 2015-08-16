using System;
using System.ComponentModel;
using VisualCrypt.Cryptography.Portable.Apps.Services;

namespace VisualCrypt.Windows.Services
{
    public class LifeTimeService : ILifeTimeService
    {
        public void HandleExitRequested(CancelEventArgs e, Func<bool> confirmDiscard)
        {
            throw new NotImplementedException();
        }
    }
}