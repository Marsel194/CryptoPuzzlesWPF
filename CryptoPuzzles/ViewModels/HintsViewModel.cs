using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    public class HintsViewModel : EntityViewModelBase<AHint, AHintCreate, AHintUpdate>
    {
        private readonly PuzzleApiService _puzzleApi;
        private ObservableCollection<APuzzle> _puzzles;

        public HintsViewModel(HintApiService hintApi, PuzzleApiService puzzleApi) : base(hintApi)
        {
            _puzzleApi = puzzleApi;
            LoadPuzzlesAsync();
        }

        public ObservableCollection<APuzzle> Puzzles { get => _puzzles; set => SetProperty(ref _puzzles, value); }

        private async Task LoadPuzzlesAsync()
        {
            Puzzles = new ObservableCollection<APuzzle>(await _puzzleApi.GetAllAsync());
        }

        protected override AHint CreateNewItem()
        {
            return new AHint(0, 0, "", "", 1, DateTime.Now);
        }

        protected override AHintCreate MapToCreateDto(AHint item)
        {
            return new AHintCreate(item.PuzzleId, item.HintText, item.HintOrder);
        }

        protected override AHintUpdate MapToUpdateDto(AHint item)
        {
            return new AHintUpdate(item.Id, item.PuzzleId, item.HintText, item.HintOrder);
        }

        protected override int GetId(AHint item) => item.Id;

        protected override async Task AddAsync()
        {
            if (NewItem.PuzzleId <= 0)
            {
                await DialogService.ShowError("Выберите головоломку!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem.HintText))
            {
                await DialogService.ShowError("Текст подсказки не может быть пустым!");
                return;
            }
            if (NewItem.HintOrder <= 0)
            {
                await DialogService.ShowError("Порядок должен быть положительным числом!");
                return;
            }

            var puzzle = Puzzles.FirstOrDefault(p => p.Id == NewItem.PuzzleId);
            string puzzleTitle = puzzle?.Title ?? "";

            var itemToAdd = new AHint(0, NewItem.PuzzleId, puzzleTitle, NewItem.HintText, NewItem.HintOrder, DateTime.Now);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();
            HasChanges = true;
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            foreach (var item in _addedItems)
            {
                if (item.PuzzleId <= 0 || string.IsNullOrWhiteSpace(item.HintText) || item.HintOrder <= 0)
                {
                    await DialogService.ShowError("Заполните все поля корректно!");
                    return;
                }
            }
            foreach (var item in Items.Except(_addedItems))
            {
                if (item.PuzzleId <= 0 || string.IsNullOrWhiteSpace(item.HintText) || item.HintOrder <= 0)
                {
                    await DialogService.ShowError("Заполните все поля корректно!");
                    return;
                }
            }

            await base.SaveAsync();
        }

        protected override bool IsEqual(AHint x, AHint y)
        {
            return x.Id == y.Id &&
                   x.PuzzleId == y.PuzzleId &&
                   x.HintText == y.HintText &&
                   x.HintOrder == y.HintOrder;
        }
    }
}