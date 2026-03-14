using Microsoft.Win32;
using System.Windows;

namespace CryptoPuzzles.Services
{
    public static class DialogService
    {
        public static Task ShowMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        public static Task ShowWarning(string message)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            return Task.CompletedTask;
        }

        public static Task ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return Task.CompletedTask;
        }

        public static async Task<bool> ShowConfirmation(string message, string confirm = "Подтверждение")
        {
            return MessageBox.Show(message, confirm, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
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