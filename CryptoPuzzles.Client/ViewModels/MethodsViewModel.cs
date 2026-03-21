using CryptoPuzzles.Client.Services;
using CryptoPuzzles.Client.Services.ApiService;
using CryptoPuzzles.Client.ViewModels.Base;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Client.ViewModels
{
    public class MethodsViewModel : EntityViewModelBase<AEncryptionMethod, AEncryptionMethodCreate, AEncryptionMethodUpdate>
    {
        private bool _showDeleted = true;

        public MethodsViewModel(EncryptionMethodApiService apiService) : base(apiService) { }

        public bool ShowDeleted
        {
            get => _showDeleted;
            set
            {
                if (SetProperty(ref _showDeleted, value))
                    ApplyFilter();
            }
        }

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
            return new AEncryptionMethodUpdate(item.Id, item.Name, item.IsDeleted, item.DeletedAt);
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

        protected override bool IsEqual(AEncryptionMethod x, AEncryptionMethod y)
        {
            return x.Id == y.Id &&
                x.Name == y.Name &&
                x.IsDeleted == y.IsDeleted &&
                x.DeletedAt == y.DeletedAt;
        }

        protected override bool HasAdditionalFilters() => !ShowDeleted;

        protected override bool FilterPredicate(AEncryptionMethod item)
        {
            if (!ShowDeleted && item.IsDeleted)
                return false;

            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            return item.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
        }
    }
}