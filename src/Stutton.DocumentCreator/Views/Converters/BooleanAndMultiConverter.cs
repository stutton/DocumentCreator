using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Stutton.DocumentCreator.Views.Converters
{
    internal sealed class BooleanAndMultiConverter
        : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.OfType<bool>().All(p => p);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}