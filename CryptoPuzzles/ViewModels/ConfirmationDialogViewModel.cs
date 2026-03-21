using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using CryptoPuzzles.Client.ViewModels.Base;

namespace CryptoPuzzles.Client.ViewModels
{
    public class ConfirmationDialogViewModel
    {
        public string Title { get; }
        public string Message { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public ConfirmationDialogViewModel(string message, string title = "Подтверждение")
        {
            Message = message;
            Title = title;
            ConfirmCommand = new AsyncRelayCommand(Confirm);
            CancelCommand = new AsyncRelayCommand(Cancel);
        }

        private Task Confirm()
        {
            DialogHost.CloseDialogCommand.Execute(true, null);
            return Task.CompletedTask;
        }

        private Task Cancel()
        {
            DialogHost.CloseDialogCommand.Execute(false, null);
            return Task.CompletedTask;
        }
    }
}