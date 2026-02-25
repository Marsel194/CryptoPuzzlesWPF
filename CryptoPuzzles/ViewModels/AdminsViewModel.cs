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
        private AAdminDto Admins
        {
            get => _admins;
            set { _admins = value;
                OnPropertyChanged();
            }
        }

        public AdminsViewModel()
        {
            _apiService = App.Services.GetService<ApiService>();
        }

        private void LoadAdmins()
        {
            
        }
    }
}
