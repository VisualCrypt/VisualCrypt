using System;
using System.Threading.Tasks;

namespace VisualCrypt.Applications.Extensions
{
    public static class TaskFireAndForget
    {
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception)
            {
                
            }
        }
    }
}
