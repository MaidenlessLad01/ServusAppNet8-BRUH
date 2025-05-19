using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServusAppNet8.Converter
{
    public class DateOnlyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime date)
                return date.ToString("MMMM dd, yyyy"); // Customize format as needed
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return DateTime.TryParse(value?.ToString(), out var date) ? date : default;
        }
    }
}
