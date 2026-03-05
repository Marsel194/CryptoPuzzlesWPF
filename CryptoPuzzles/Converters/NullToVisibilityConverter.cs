using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CryptoPuzzles.Helpers
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Если параметр "invert" – инвертируем логику
            bool invert = parameter?.ToString()?.ToLower() == "invert";
            bool isNull = value == null;
            bool visible = invert ? !isNull : isNull;
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}