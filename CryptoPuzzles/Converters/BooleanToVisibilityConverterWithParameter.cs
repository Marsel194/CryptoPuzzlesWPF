using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CryptoPuzzles.Converters
{
    public class BooleanToVisibilityConverterWithParameter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return Visibility.Collapsed;

            bool invert = false;
            if (parameter is string paramString)
            {
                string param = paramString.ToLowerInvariant();
                invert = param is "true" or "invert" or "1" or "yes";
            }

            bool result = invert ? !boolValue : boolValue;
            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}