using CryptoPuzzles.SharedDTO;
using Microsoft.Extensions.DependencyInjection;
using CryptoPuzzles.ViewModels.Base;
using CryptoPuzzles.Services;

namespace CryptoPuzzles.ViewModels
{
    internal class AdminsViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private AAdminDto? _admins;
        private AAdminDto? Admins
        {
            get => _admins;
            set { _admins = value;
                OnPropertyChanged();
            }
        }

        public AdminsViewModel()
        {
            LoadAdmins();
            _apiService = App.Services.GetService<ApiService>()
            ?? throw new Exception("ApiService not registered");
        }

        private async Task LoadAdmins()
        {
            Admins = await _apiService.GetAdmins();
        }
    }
}
