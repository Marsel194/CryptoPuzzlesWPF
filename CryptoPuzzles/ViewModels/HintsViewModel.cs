using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    public class HintsViewModel : EntityViewModelBase<AHint, AHintCreate, AHintUpdate>
    {
        private readonly PuzzleApiService _puzzleApi;
        private ObservableCollection<APuzzle> _puzzles = [];
        private bool _showDeleted = true;
        private APuzzle? _selectedPuzzleFilter;
        private int? _minHintOrder;
        private int? _maxHintOrder;
        private DateTime? _minCreatedAt;
        private DateTime? _maxCreatedAt;

        public HintsViewModel(HintApiService hintApi, PuzzleApiService puzzleApi) : base(hintApi)
        {
            _puzzleApi = puzzleApi;
            _ = LoadPuzzlesAsync();
        }

        public ObservableCollection<APuzzle> Puzzles { get => _puzzles; set => SetProperty(ref _puzzles, value); }

        public bool ShowDeleted
        {
            get => _showDeleted;
            set
            {
                if (SetProperty(ref _showDeleted, value))
                    ApplyFilter();
            }
        }

        public APuzzle? SelectedPuzzleFilter
        {
            get => _selectedPuzzleFilter;
            set
            {
                if (SetProperty(ref _selectedPuzzleFilter, value))
                    ApplyFilter();
            }
        }

        public int? MinHintOrder
        {
            get => _minHintOrder;
            set
            {
                if (SetProperty(ref _minHintOrder, value))
                    ApplyFilter();
            }
        }

        public int? MaxHintOrder
        {
            get => _maxHintOrder;
            set
            {
                if (SetProperty(ref _maxHintOrder, value))
                    ApplyFilter();
            }
        }

        public DateTime? MinCreatedAt
        {
            get => _minCreatedAt;
            set
            {
                if (SetProperty(ref _minCreatedAt, value))
                    ApplyFilter();
            }
        }

        public DateTime? MaxCreatedAt
        {
            get => _maxCreatedAt;
            set
            {
                if (SetProperty(ref _maxCreatedAt, value))
                    ApplyFilter();
            }
        }

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
            return new AHintUpdate(item.Id, item.PuzzleId, item.HintText, item.HintOrder, item.IsDeleted, item.DeletedAt);
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
            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();
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
                   x.HintOrder == y.HintOrder &&
                   x.IsDeleted == y.IsDeleted &&
                   x.DeletedAt == y.DeletedAt;
        }

        protected override bool HasAdditionalFilters() =>
            !ShowDeleted || SelectedPuzzleFilter != null || MinHintOrder.HasValue ||
            MaxHintOrder.HasValue || MinCreatedAt.HasValue || MaxCreatedAt.HasValue;

        protected override bool FilterPredicate(AHint item)
        {
            if (!ShowDeleted && item.IsDeleted)
                return false;

            if (SelectedPuzzleFilter != null && item.PuzzleId != SelectedPuzzleFilter.Id)
                return false;

            bool orderMatch = true;
            if (MinHintOrder.HasValue)
                orderMatch = item.HintOrder >= MinHintOrder.Value;
            if (orderMatch && MaxHintOrder.HasValue)
                orderMatch = item.HintOrder <= MaxHintOrder.Value;

            bool createdMatch = true;
            if (MinCreatedAt.HasValue)
                createdMatch = item.CreatedAt >= MinCreatedAt.Value;
            if (createdMatch && MaxCreatedAt.HasValue)
                createdMatch = item.CreatedAt <= MaxCreatedAt.Value;

            if (!orderMatch || !createdMatch)
                return false;

            if (string.IsNullOrWhiteSpace(FilterText)) return true;

            var f = FilterText.ToLower();
            return item.PuzzleTitle.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                   item.HintText.Contains(f, StringComparison.OrdinalIgnoreCase);
        }
    }
}