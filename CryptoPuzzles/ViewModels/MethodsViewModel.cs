using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class MethodsViewModel : EntityViewModelBase<AEncryptionMethod, AEncryptionMethodCreate, AEncryptionMethodUpdate>
    {
        public MethodsViewModel(EncryptionMethodApiService apiService) : base(apiService) { }

        protected override AEncryptionMethod CreateNewItem()
        {
            return new AEncryptionMethod(0, "");
        }

        protected override AEncryptionMethodCreate MapToCreateDto(AEncryptionMethod item)
        {
            return new AEncryptionMethodCreate(item.Name);
        }

        protected override AEncryptionMethodUpdate MapToUpdateDto(AEncryptionMethod item)
        {
            return new AEncryptionMethodUpdate(item.Id, item.Name);
        }

        protected override int GetId(AEncryptionMethod item) => item.Id;

        protected override async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(NewItem?.Name))
            {
                await DialogService.ShowError("Название метода не может быть пустым!");
                return;
            }

            var itemToAdd = new AEncryptionMethod(0, NewItem.Name);

            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();

            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            foreach (var item in _addedItems)
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    await DialogService.ShowError("Название метода не может быть пустым!");
                    return;
                }
            }

            foreach (var item in Items.Except(_addedItems))
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    await DialogService.ShowError("Название метода не может быть пустым!");
                    return;
                }
            }

            await base.SaveAsync();
        }

        protected override bool FilterPredicate(AEncryptionMethod item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            return item.Name.ToLower().Contains(FilterText.ToLower());
        }
    }
}