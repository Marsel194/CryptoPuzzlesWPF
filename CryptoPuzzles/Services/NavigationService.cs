using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoPuzzles.Services
{
    public class NavigationService
    {
        public event Action<ViewModelBase>? OnViewChanged;

        public void NavigateTo<T>() where T : ViewModelBase
        {
            var viewModel = App.Services.GetService<T>() ??
                throw new InvalidOperationException($"ViewModel {typeof(T).Name} не зарегистрирован в DI");
            OnViewChanged?.Invoke(viewModel);
        }
    }
}