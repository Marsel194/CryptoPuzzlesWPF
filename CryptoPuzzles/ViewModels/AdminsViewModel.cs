using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    internal class AdminsViewModel : ViewModelBase
    {
        private readonly AdminApiService _apiService;

        private ObservableCollection<AAdmin>? _admins;
        public ObservableCollection<AAdmin>? Admins
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
            _apiService = App.Services.GetService<AdminApiService>()
                ?? throw new Exception("ApiService not registered");

            Admins = [];

            _ = LoadAdminsAsync();
        }

        private async Task LoadAdminsAsync()
        {
            try
            {
                var adminsList = await _apiService.GetAdminsAsync();
                Admins = new ObservableCollection<AAdmin>(adminsList);
            }
            catch (Exception ex)
            {
                DialogService.ShowError("Ошибка обработки запроса: " + ex.Message);
            }
        }
    }
}