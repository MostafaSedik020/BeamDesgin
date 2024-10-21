using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BeamDesgin.UI
{
    public class RebarSizeToColorConverter : IValueConverter
    {
        private static readonly HashSet<int> RebarSizes = new HashSet<int> { 12, 14, 16, 18, 20, 22, 25 };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int size && RebarSizes.Contains(size))
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cdd9ea"));
            }
            return Brushes.White; // Default background color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
