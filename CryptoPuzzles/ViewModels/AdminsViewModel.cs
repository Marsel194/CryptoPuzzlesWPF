using CryptoPuzzles.Services;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    internal class AdminsViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;

        private ObservableCollection<AAdminDto>? _admins;
        public ObservableCollection<AAdminDto>? Admins
        {
            get => _admins;
            set
            {
                _admins = value;
                OnPropertyChanged();
            }
        }

        public AdminsViewModel()
        {
            _apiService = App.Services.GetService<ApiService>()
                ?? throw new Exception("ApiService not registered");

            Admins = [];

            _ = LoadAdminsAsync();
        }

        private async Task LoadAdminsAsync()
        {
            try
            {
                var adminsList = await _apiService.GetAdmins();
                Admins = new ObservableCollection<AAdminDto>(adminsList);
            }
            catch (Exception ex)
            {
                DialogService.ShowError("Ошибка обработки запроса: " + ex.Message);
            }
        }
    }
}