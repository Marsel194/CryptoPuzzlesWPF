using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoPuzzles.Services
{
    public class AdminNavigationService
    {
        private readonly IServiceProvider _services;
        private readonly Stack<ViewModelBase> _backStack = new();
        private readonly Stack<ViewModelBase> _forwardStack = new();
        private ViewModelBase _currentViewModel;

        public AdminNavigationService(IServiceProvider services)
        {
            _services = services;
        }

        public event Action<ViewModelBase> CurrentViewModelChanged;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke(value);
            }
        }

        public bool CanGoBack => _backStack.Count > 0;
        public bool CanGoForward => _forwardStack.Count > 0;

        public void NavigateTo<T>(bool addToHistory = true) where T : ViewModelBase
        {
            var vm = _services.GetRequiredService<T>();
            NavigateTo(vm, addToHistory);
        }

        public void NavigateTo(ViewModelBase viewModel, bool addToHistory = true)
        {
            if (addToHistory && CurrentViewModel != null)
            {
                _backStack.Push(CurrentViewModel);
                _forwardStack.Clear();
            }
            CurrentViewModel = viewModel;
        }

        public void GoBack()
        {
            if (!CanGoBack) return;
            _forwardStack.Push(CurrentViewModel);
            CurrentViewModel = _backStack.Pop();
        }

        public void GoForward()
        {
            if (!CanGoForward) return;
            _backStack.Push(CurrentViewModel);
            CurrentViewModel = _forwardStack.Pop();
        }

        public void GoHome(ViewModelBase homeViewModel)
        {
            if (CurrentViewModel != null && CurrentViewModel != homeViewModel)
            {
                _backStack.Push(CurrentViewModel);
                _forwardStack.Clear();
            }
            CurrentViewModel = homeViewModel;
        }
    }
}