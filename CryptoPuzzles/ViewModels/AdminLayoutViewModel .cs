using CryptoPuzzles.Services;
using CryptoPuzzles.ViewModels.Base;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class AdminLayoutViewModel : ViewModelBase
    {
        private readonly AdminNavigationService _navigation;

        public AdminLayoutViewModel(AdminNavigationService navigation)
        {
            _navigation = navigation;
            _navigation.CurrentViewModelChanged += vm => OnPropertyChanged(nameof(CurrentViewModel));

            NavigateBackCommand = new AsyncRelayCommand(_ => Task.Run(() => _navigation.GoBack()), _ => CanGoBack);
            NavigateForwardCommand = new AsyncRelayCommand(_ => Task.Run(() => _navigation.GoForward()), _ => CanGoForward);
            NavigateHomeCommand = new AsyncRelayCommand(_ => Task.Run(() => _navigation.NavigateTo<AdminViewModel>()));

            // Начальная страница
            _navigation.NavigateTo<AdminViewModel>(addToHistory: false);
        }

        public ViewModelBase CurrentViewModel => _navigation.CurrentViewModel;
        public bool CanGoBack => _navigation.CanGoBack;
        public bool CanGoForward => _navigation.CanGoForward;

        public ICommand NavigateBackCommand { get; }
        public ICommand NavigateForwardCommand { get; }
        public ICommand NavigateHomeCommand { get; }
    }
}
