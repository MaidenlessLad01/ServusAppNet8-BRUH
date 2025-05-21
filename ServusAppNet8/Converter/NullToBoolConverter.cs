using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ServusAppNet8.Converter
{
    internal class NullToBoolConverter : IValueConverter
    {
        //Makes it so that the post will only properly show what info it has
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => 
            !string.IsNullOrEmpty(value?.ToString());

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
