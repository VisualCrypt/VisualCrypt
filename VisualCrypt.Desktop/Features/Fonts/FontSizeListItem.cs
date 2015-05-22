using System;
using System.Globalization;
using System.Windows.Controls;

namespace VisualCrypt.Desktop.Models.Fonts
{
    internal class FontSizeListItem : TextBlock, IComparable
    {
        private readonly double _sizeInPoints;

        public FontSizeListItem(double sizeInPoints)
        {
            _sizeInPoints = sizeInPoints;
            Text = sizeInPoints.ToString(CultureInfo.CurrentCulture);
        }

        public double SizeInPoints
        {
            get { return _sizeInPoints; }
        }

        public double SizeInPixels
        {
            get { return PointsToPixels(_sizeInPoints); }
        }

        int IComparable.CompareTo(object obj)
        {
            double value;

            if (obj is double)
            {
                value = (double)obj;
            }
            else
            {
                if (!double.TryParse(obj.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value))
                {
                    return 1;
                }
            }

            return
                FuzzyEqual(_sizeInPoints, value)
                    ? 0
                    : (_sizeInPoints < value) ? -1 : 1;
        }

        public override string ToString()
        {
            return _sizeInPoints.ToString(CultureInfo.CurrentCulture);
        }

        public static bool FuzzyEqual(double a, double b)
        {
            return Math.Abs(a - b) < 0.01;
        }

        public static double PointsToPixels(double value)
        {
            return value * (96.0 / 72.0);
        }

        public static double PixelsToPoints(double value)
        {
            return value * (72.0 / 96.0);
        }
    }
}