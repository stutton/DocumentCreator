using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Stutton.DocumentCreator.Views.Converters
{
    public class FlattenListConverter : IValueConverter
    {
        public char Separator { get; set; }
        public string PropertyName { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<object> ie)
            {
                var list = ie.ToList();
                if (list.Any())
                {
                    var useProperty = false;
                    var properties = list[0].GetType().GetProperties();
                    if (properties.Select(p => p.Name).Contains(PropertyName))
                    {
                        useProperty = true;
                    }

                    var sb = new StringBuilder();
                    foreach (var item in list)
                    {
                        var valueObj = useProperty ? item.GetType().GetProperty(PropertyName).GetValue(item) : item;
                        var valueStr = valueObj?.ToString() ?? string.Empty;

                        sb.Append($"{valueStr}{Separator} ");
                    }

                    return sb.ToString().TrimEnd(Separator, ' ');
                }
                return string.Empty;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
