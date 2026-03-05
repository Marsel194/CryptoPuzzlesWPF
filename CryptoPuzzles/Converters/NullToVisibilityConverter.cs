using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CryptoPuzzles.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverse = parameter as string == "inverse";
            bool isNull = value == null;
            return (inverse ? !isNull : isNull) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}