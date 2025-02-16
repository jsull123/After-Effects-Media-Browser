using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaManager2
{
    public class ScaleMultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double scale && parameter is string baseValue && double.TryParse(baseValue, out double baseDimension))
            {
                return scale * baseDimension;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
