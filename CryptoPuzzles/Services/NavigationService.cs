using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CryptoPuzzles.Services
{
    public class NavigationService
    {
        private ViewModelBase _currentView;

        public event Action<ViewModelBase> OnViewChanged;

        public async Task NavigateToAsync<T>() where T : ViewModelBase
        {
            try
            {
                var viewModel = App.Services.GetService<T>()
                     ?? throw new InvalidOperationException($"ViewModel {typeof(T).Name} не зарегистрирован в DI");

                _currentView = viewModel;
                OnViewChanged?.Invoke(viewModel);
            }
            catch (Exception ex)
            {
                // Дополнительный перехват
                System.Diagnostics.Debug.WriteLine($"Ошибка в NavigateToAsync: {ex}");
                MessageBox.Show($"Общая ошибка навигации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            await Task.CompletedTask;
        }
    }
}