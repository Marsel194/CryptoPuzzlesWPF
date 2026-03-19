using System.Windows;
using MaterialDesignThemes.Wpf;
using CryptoPuzzles.ViewModels;
using CryptoPuzzles.Views;
using Microsoft.Win32;

namespace CryptoPuzzles.Services
{
    public static class DialogService
    {
        public static Task ShowMessage(string message)
        {
            NotificationService.ShowInfo(message);
            return Task.CompletedTask;
        }

        public static Task ShowWarning(string message)
        {
            NotificationService.ShowWarning(message);
            return Task.CompletedTask;
        }

        public static Task ShowError(string message)
        {
            NotificationService.ShowError(message);
            return Task.CompletedTask;
        }

        public static async Task<bool> ShowConfirmation(string message, string title = "Подтверждение")
        {
            var viewModel = new ConfirmationDialogViewModel(message, title);
            var view = new ConfirmationDialog { DataContext = viewModel };
            var result = await DialogHost.Show(view, "RootDialogHost");
            return result is bool b && b;
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