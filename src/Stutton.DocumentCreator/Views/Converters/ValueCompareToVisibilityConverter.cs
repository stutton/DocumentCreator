using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Stutton.DocumentCreator.Views.Converters
{
    public class ValueCompareToVisibilityConverter : ValueCompareConverterBase<Visibility>
    {
        public ValueCompareToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }
    }
}
