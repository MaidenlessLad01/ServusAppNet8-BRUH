using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServusAppNet8.Converter
{
    internal class NullToBoolConverter : IValueConverter
    {
        //Makes it that if the if the caption or post is empty, then it doesn't show it to the user
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => 
            !string.IsNullOrEmpty(value?.ToString());

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
