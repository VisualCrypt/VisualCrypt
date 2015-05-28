using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VisualCrypt.Desktop.Shared.Commands
{
    public static class EditorCommands
    {
        public static readonly RoutedUICommand Print = new RoutedUICommand();

        // Menu Edit
        public static readonly RoutedUICommand Find = new RoutedUICommand();
        public static readonly RoutedUICommand FindNext = new RoutedUICommand();
        public static readonly RoutedUICommand FindPrevious = new RoutedUICommand();
        public static readonly RoutedUICommand Replace = new RoutedUICommand();
        public static readonly RoutedUICommand DeleteLine = new RoutedUICommand();
        public static readonly RoutedUICommand GoTo = new RoutedUICommand();
        public static readonly RoutedUICommand InsertDate = new RoutedUICommand();

        public static readonly RoutedUICommand Font = new RoutedUICommand();
        public static readonly RoutedUICommand ZoomIn = new RoutedUICommand();
        public static readonly RoutedUICommand ZoomOut = new RoutedUICommand();
        public static readonly RoutedUICommand Zoom100 = new RoutedUICommand();
        

    }
}
