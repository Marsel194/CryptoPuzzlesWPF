using System.Globalization;
using System.Windows.Data;

namespace CryptoPuzzles.Converters
{
    public class BusyToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string param)
            {
                var parts = param.Split('|');
                if (parts.Length == 2)
                {
                    return (value is bool busy && busy) ? parts[1] : parts[0];
                }
            }
            return value is bool b && b ? "Загрузка..." : "ВОЙТИ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}