using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FreedomClient.Converters
{
    public class NegateNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intV)
            {
                return -intV;
            }
            if (value is double doubleV)
            {
                return -doubleV;
            }
            if (value is decimal decimalV)
            {
                return -decimalV;
            }
            if (value is float floatV)
            {
                return -floatV;
            }
            if (value is byte byteV)
            {
                return -byteV;
            }
            if (value is short shortV)
            {
                return -shortV;
            }
            if (value is long longV)
            {
                return -longV;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(NegateNumberConverter)} is a OneWay converter.");
        }
    }
}
