using CryptoPuzzles.SharedDTO;
using Hairulin_02_01.Services;
using Hairulin_02_01.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Hairulin_02_01.ViewModels
{
    internal class AdminsViewModel : ViewModelBase
    {
        private ApiService _apiService;
        private AAdminDto _admins;
        public AAdminDto Admins
        {
            get => _admins;
            set { _admins = value;
                OnPropertyChanged();
            }
        }

        public AdminsViewModel()
        {
            _apiService = App.Services.GetService<ApiService>();

            LoadAdmins();
        }

        private async Task LoadAdmins()
        {
            Admins = await _apiService.GetAdmins();
        }
    }
}
