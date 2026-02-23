using Hairulin_02_01.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Hairulin_02_01.Services
{
    public class NavigationService
    {
        public event Action<ViewModelBase> OnViewChanged;

        public void NavigateTo<T>() where T : ViewModelBase
        {
            var viewModel = App.Services.GetService<T>();

            OnViewChanged?.Invoke(viewModel);
        }
    }
}