using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.ViewModels
{
    public class AdminsViewModel : EntityViewModelBase<AAdmin, AAdminCreate, AAdminUpdate>
    {
        private string _newPassword = string.Empty;

        public AdminsViewModel(AdminApiService apiService) : base(apiService) { }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        protected override AAdmin CreateNewItem()
        {
            return new AAdmin(0, "", "", "", null, DateTime.Now);
        }

        protected override AAdminCreate MapToCreateDto(AAdmin item)
        {
            return new AAdminCreate(item.Login, NewPassword, item.FirstName, item.LastName, item.MiddleName);
        }

        protected override AAdminUpdate MapToUpdateDto(AAdmin item)
        {
            return new AAdminUpdate(item.Id, item.Login, item.FirstName, item.LastName, item.MiddleName,
                string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword);
        }

        protected override int GetId(AAdmin item) => item.Id;

        protected override async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(NewItem?.Login))
            {
                DialogService.ShowError("Логин не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.FirstName))
            {
                DialogService.ShowError("Имя не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.LastName))
            {
                DialogService.ShowError("Фамилия не может быть пустой!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                DialogService.ShowError("Пароль обязателен при создании!");
                return;
            }

            var itemToAdd = new AAdmin(0, NewItem.Login, NewItem.FirstName, NewItem.LastName,
                NewItem.MiddleName, DateTime.Now);
            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();
            NewPassword = string.Empty;
            HasChanges = true;
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            foreach (var item in _addedItems)
            {
                if (string.IsNullOrWhiteSpace(item.Login) ||
                    string.IsNullOrWhiteSpace(item.FirstName) ||
                    string.IsNullOrWhiteSpace(item.LastName))
                {
                    DialogService.ShowError("Заполните все обязательные поля (логин, имя, фамилия)!");
                    return;
                }
            }

            foreach (var item in Items.Except(_addedItems))
            {
                if (string.IsNullOrWhiteSpace(item.Login) ||
                    string.IsNullOrWhiteSpace(item.FirstName) ||
                    string.IsNullOrWhiteSpace(item.LastName))
                {
                    DialogService.ShowError("Заполните все обязательные поля (логин, имя, фамилия)!");
                    return;
                }
            }

            await base.SaveAsync();
            NewPassword = string.Empty;
        }

        protected override bool IsEqual(AAdmin x, AAdmin y)
        {
            return x.Id == y.Id &&
                   x.Login == y.Login &&
                   x.FirstName == y.FirstName &&
                   x.LastName == y.LastName &&
                   x.MiddleName == y.MiddleName;
        }
    }
}