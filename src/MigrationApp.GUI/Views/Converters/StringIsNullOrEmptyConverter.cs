using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationApp.GUI.Views.Converters
{
    public class StringIsNullOrEmptyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var input = value as string;
            var defaultMessage = parameter as string ?? "Default value"; // Use the parameter for the default message
            return string.IsNullOrEmpty(input) ? defaultMessage : input;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
