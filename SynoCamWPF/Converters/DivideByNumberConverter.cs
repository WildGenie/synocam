using System;
using System.Globalization;
using System.Windows.Data;

namespace SynoCamWPF.Converters
{
    class DivideByNumberConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                throw new ArgumentException("This converter only accepts 2 parameters");

            double divided = (double)values[0] / (int)values[1];
            return divided;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
