using System;
using System.Globalization;
using System.Windows.Data;

namespace Stutton.DocumentCreator.Views.Converters
{
    public class ValueCompareConverterBase<T> : IValueConverter
    {
        public object CompareToValue { get; set; }
        public bool IsInverted { get; set; }
        public T TrueValue { get; set; }
        public T FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return GetReturnValue(CompareToValue == null);

            var result = Equals(value, CompareToValue);
            return GetReturnValue(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private T GetReturnValue(bool converterResult)
        {
            if (IsInverted)
                converterResult = !converterResult;
            return converterResult ? TrueValue : FalseValue;
        }
    }
}