using Hairulin_02_01.ViewModels.Base;
using System.Windows.Input;

namespace Hairulin_02_01.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand NavigateToAdminCommand { get; }

        public MainViewModel()
        {
            NavigateToLoginCommand = new RelayCommand(_ => NavigateTo<LoginView, LoginViewModel>());
            NavigateToRegisterCommand = new RelayCommand(_ => NavigateTo<RegisterView, RegisterViewModel>());
            NavigateToAdminCommand = new RelayCommand(_ => NavigateTo<AdminView, AdminViewModel>());

            NavigateTo<LoginView, LoginViewModel>();
        }

        private void NavigateTo<TView, TViewModel>(params object[] parameters)
            where TView : new()
            where TViewModel : class
        {
            var viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), parameters);

            var view = new TView();
            if (view is System.Windows.FrameworkElement element)
                element.DataContext = viewModel;

            CurrentView = view;
        }
    }
}