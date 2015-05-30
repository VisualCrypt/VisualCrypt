using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace VisualCrypt.Desktop.Shared
{
    public static class DelegateCommandExtensions
    {
        public static DelegateCommand Create(DelegateCommand command)
        {
            return new DelegateCommand(null);
        }
    }
}
