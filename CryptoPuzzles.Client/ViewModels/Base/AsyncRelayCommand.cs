using System.Windows.Input;

namespace CryptoPuzzles.Client.ViewModels.Base
{
    public class AsyncRelayCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null) : ICommand
    {
        private readonly Func<object?, Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Func<object?, bool>? _canExecute = canExecute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<Task> execute)
            : this(_ => execute()) { }

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
            : this(_ => execute(), canExecute is null ? null : _ => canExecute())
        {
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) =>
            !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _isExecuting = true;
                CommandManager.InvalidateRequerySuggested();
                await _execute(parameter);
            }
            finally
            {
                _isExecuting = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public static void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class AsyncRelayCommand<T>(Func<T?, Task> execute, Func<T?, bool>? canExecute = null) : ICommand
    {
        private readonly Func<T?, Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Func<T?, bool>? _canExecute = canExecute;
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            if (_isExecuting) return false;

            if (parameter is T typed)
                return _canExecute?.Invoke(typed) ?? true;

            return _canExecute?.Invoke(default) ?? true;
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                _isExecuting = true;
                CommandManager.InvalidateRequerySuggested();

                if (parameter is T typed)
                    await _execute(typed);
                else
                    await _execute(default);
            }
            finally
            {
                _isExecuting = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public static void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}