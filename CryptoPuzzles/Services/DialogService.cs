using Microsoft.Win32;
using System.Windows;

namespace Hairulin_02_01.Services
{
    public class DialogService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Информация",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "Предупреждение",
                           MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка",
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowConfirmation(string message)
        {
            return MessageBox.Show(message, "Подтверждение",
                                  MessageBoxButton.YesNo, MessageBoxImage.Question)
                   == MessageBoxResult.Yes;
        }

        public T ShowDialog<T>(object viewModel) where T : Window, new()
        {
            var window = new T();
            window.DataContext = viewModel;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
            return window;
        }

        public string ShowOpenFileDialog()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Все файлы (*.*)|*.*";

            if (dialog.ShowDialog() == true)
                return dialog.FileName;

            return null;
        }
    }
}