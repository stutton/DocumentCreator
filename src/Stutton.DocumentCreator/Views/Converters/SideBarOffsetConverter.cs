using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Stutton.DocumentCreator.Views.Converters
{
    public sealed class SideBarOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value as double? ?? 0;
            if (double.IsInfinity(d) || double.IsNaN(d)) d = 0;

            return new Thickness(0 - d, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}