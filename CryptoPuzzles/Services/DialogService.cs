using Microsoft.Win32;
using System.Windows;

namespace CryptoPuzzles.Services
{
    public class DialogService
    {
        public static void ShowMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool ShowConfirmation(string message)
        {
            return MessageBox.Show(message, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question)
                   == MessageBoxResult.Yes;
        }

        public static T ShowDialog<T>(object viewModel) where T : Window, new()
        {
            var window = new T
            {
                DataContext = viewModel,
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
            return window;
        }

        public static string? ShowOpenFileDialog(string filter = "Все файлы (*.*)|*.*")
        {
            var dialog = new OpenFileDialog { Filter = filter };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}