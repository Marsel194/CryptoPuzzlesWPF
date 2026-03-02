using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.ViewModels
{
    public class UsersViewModel : EntityViewModelBase<AUser, AUserCreate, AUserUpdate>
    {
        private string _newPassword = string.Empty;

        public UsersViewModel(UserApiService apiService) : base(apiService) { }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        protected override AUser CreateNewItem()
        {
            return new AUser(0, "", "", "", DateTime.Now);
        }

        protected override AUserCreate MapToCreateDto(AUser item)
        {
            return new AUserCreate(item.Login, NewPassword, item.Username, item.Email);
        }

        protected override AUserUpdate MapToUpdateDto(AUser item)
        {
            // Если NewPassword не пуст, передаём его, иначе null (не меняем)
            return new AUserUpdate(item.Id, item.Login, item.Username, item.Email,
                string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword);
        }

        protected override int GetId(AUser item) => item.Id;

        protected override async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(NewItem?.Login))
            {
                DialogService.ShowError("Логин не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.Username))
            {
                DialogService.ShowError("Имя пользователя не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.Email))
            {
                DialogService.ShowError("Email не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                DialogService.ShowError("Пароль обязателен при создании!");
                return;
            }

            var itemToAdd = new AUser(0, NewItem.Login, NewItem.Username, NewItem.Email, DateTime.Now);
            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();
            NewPassword = string.Empty; // сброс пароля
            HasChanges = true;
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            // Проверка добавляемых
            foreach (var item in _addedItems)
            {
                if (string.IsNullOrWhiteSpace(item.Login) ||
                    string.IsNullOrWhiteSpace(item.Username) ||
                    string.IsNullOrWhiteSpace(item.Email))
                {
                    DialogService.ShowError("Все поля должны быть заполнены!");
                    return;
                }
                // Пароль уже проверен при добавлении, но можно перепроверить
            }

            // Проверка изменённых
            foreach (var item in Items.Except(_addedItems))
            {
                if (string.IsNullOrWhiteSpace(item.Login) ||
                    string.IsNullOrWhiteSpace(item.Username) ||
                    string.IsNullOrWhiteSpace(item.Email))
                {
                    DialogService.ShowError("Все поля должны быть заполнены!");
                    return;
                }
            }

            await base.SaveAsync();
            // После успешного сохранения сбрасываем пароль (если он был введён для редактирования)
            NewPassword = string.Empty;
        }

        protected override bool IsEqual(AUser x, AUser y)
        {
            return x.Id == y.Id &&
                   x.Login == y.Login &&
                   x.Username == y.Username &&
                   x.Email == y.Email;
        }
    }
}