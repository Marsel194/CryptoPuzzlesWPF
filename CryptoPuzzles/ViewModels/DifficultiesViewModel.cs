using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class DifficultiesViewModel : EntityViewModelBase<ADifficulty, ADifficultyCreate, ADifficultyUpdate>
    {
        public DifficultiesViewModel(DifficultyApiService apiService) : base(apiService) { }

        protected override ADifficulty CreateNewItem()
        {
            return new ADifficulty(0, string.Empty);
        }

        protected override ADifficultyCreate MapToCreateDto(ADifficulty item)
        {
            return new ADifficultyCreate(item.DifficultyName);
        }

        protected override ADifficultyUpdate MapToUpdateDto(ADifficulty item)
        {
            return new ADifficultyUpdate(item.Id, item.DifficultyName);
        }

        protected override int GetId(ADifficulty item) => item.Id;

        protected override async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(NewItem?.DifficultyName))
            {
                await DialogService.ShowError("Название сложности не может быть пустым!");
                return;
            }

            var itemToAdd = new ADifficulty(0, NewItem.DifficultyName);
            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();
            HasChanges = true;
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            foreach (var item in _addedItems)
            {
                if (string.IsNullOrWhiteSpace(item.DifficultyName))
                {
                    await DialogService.ShowError("Название сложности не может быть пустым!");
                    return;
                }
            }

            foreach (var item in Items.Except(_addedItems))
            {
                if (string.IsNullOrWhiteSpace(item.DifficultyName))
                {
                    await DialogService.ShowError("Название сложности не может быть пустым!");
                    return;
                }
            }

            await base.SaveAsync();
        }

        protected override bool IsEqual(ADifficulty x, ADifficulty y)
        {
            return x.Id == y.Id && x.DifficultyName == y.DifficultyName;
        }
    }
}