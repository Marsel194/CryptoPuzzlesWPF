using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    public class TutorialsViewModel : EntityViewModelBase<ATutorial, ATutorialCreate, ATutorialUpdate>
    {
        private readonly EncryptionMethodApiService _methodApi;
        private ObservableCollection<AEncryptionMethod> _methods = [];
        private bool _showDeleted = true;
        private AEncryptionMethod? _selectedMethodFilter;
        private string _titleFilter = string.Empty;
        private int? _minSortOrder;
        private int? _maxSortOrder;
        private DateTime? _minCreatedAt;
        private DateTime? _maxCreatedAt;

        public TutorialsViewModel(TutorialApiService tutorialApi, EncryptionMethodApiService methodApi) : base(tutorialApi)
        {
            _methodApi = methodApi;
            _ = LoadMethodsAsync();
        }

        public ObservableCollection<AEncryptionMethod> Methods { get => _methods; set => SetProperty(ref _methods, value); }

        public bool ShowDeleted
        {
            get => _showDeleted;
            set
            {
                if (SetProperty(ref _showDeleted, value))
                    ApplyFilter();
            }
        }

        public AEncryptionMethod? SelectedMethodFilter
        {
            get => _selectedMethodFilter;
            set
            {
                if (SetProperty(ref _selectedMethodFilter, value))
                    ApplyFilter();
            }
        }

        public string TitleFilter
        {
            get => _titleFilter;
            set
            {
                if (SetProperty(ref _titleFilter, value))
                    ApplyFilter();
            }
        }

        public int? MinSortOrder
        {
            get => _minSortOrder;
            set
            {
                if (SetProperty(ref _minSortOrder, value))
                    ApplyFilter();
            }
        }

        public int? MaxSortOrder
        {
            get => _maxSortOrder;
            set
            {
                if (SetProperty(ref _maxSortOrder, value))
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

        private async Task LoadMethodsAsync()
        {
            Methods = new ObservableCollection<AEncryptionMethod>(await _methodApi.GetAllAsync());
        }

        protected override ATutorial CreateNewItem()
        {
            return new ATutorial(0, 0, "", "", "", 0, DateTime.Now);
        }

        protected override ATutorialCreate MapToCreateDto(ATutorial item)
        {
            return new ATutorialCreate(item.MethodId, item.TheoryTitle, item.TheoryContent, item.SortOrder);
        }

        protected override ATutorialUpdate MapToUpdateDto(ATutorial item)
        {
            return new ATutorialUpdate(item.Id, item.MethodId, item.TheoryTitle, item.TheoryContent, item.SortOrder, item.IsDeleted, item.DeletedAt);
        }

        protected override int GetId(ATutorial item) => item.Id;

        protected override async Task AddAsync()
        {
            if (NewItem.MethodId <= 0)
            {
                await DialogService.ShowError("Выберите метод!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem.TheoryTitle))
            {
                await DialogService.ShowError("Заголовок не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem.TheoryContent))
            {
                await DialogService.ShowError("Содержание не может быть пустым!");
                return;
            }

            var itemToAdd = new ATutorial(0, NewItem.MethodId, "", NewItem.TheoryTitle, NewItem.TheoryContent,
                 NewItem.SortOrder, DateTime.Now);
            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            foreach (var item in _addedItems)
            {
                if (item.MethodId <= 0 || string.IsNullOrWhiteSpace(item.TheoryTitle) || string.IsNullOrWhiteSpace(item.TheoryContent))
                {
                    await DialogService.ShowError("Заполните все обязательные поля!");
                    return;
                }
            }
            foreach (var item in Items.Except(_addedItems))
            {
                if (item.MethodId <= 0 || string.IsNullOrWhiteSpace(item.TheoryTitle) || string.IsNullOrWhiteSpace(item.TheoryContent))
                {
                    await DialogService.ShowError("Заполните все обязательные поля!");
                    return;
                }
            }

            await base.SaveAsync();
        }

        protected override bool IsEqual(ATutorial x, ATutorial y)
        {
            return x.Id == y.Id &&
                   x.MethodId == y.MethodId &&
                   x.TheoryTitle == y.TheoryTitle &&
                   x.TheoryContent == y.TheoryContent &&
                   x.SortOrder == y.SortOrder &&
                   x.IsDeleted == y.IsDeleted &&
                   x.DeletedAt == y.DeletedAt;
        }

        protected override bool HasAdditionalFilters() =>
            !ShowDeleted || SelectedMethodFilter != null || !string.IsNullOrWhiteSpace(TitleFilter) ||
            MinSortOrder.HasValue || MaxSortOrder.HasValue || MinCreatedAt.HasValue || MaxCreatedAt.HasValue;

        protected override bool FilterPredicate(ATutorial item)
        {
            if (!ShowDeleted && item.IsDeleted)
                return false;

            if (SelectedMethodFilter != null && item.MethodId != SelectedMethodFilter.Id)
                return false;

            bool titleMatch = string.IsNullOrWhiteSpace(TitleFilter) ||
                              item.TheoryTitle.Contains(TitleFilter, StringComparison.OrdinalIgnoreCase);

            bool orderMatch = true;
            if (MinSortOrder.HasValue)
                orderMatch = item.SortOrder >= MinSortOrder.Value;
            if (orderMatch && MaxSortOrder.HasValue)
                orderMatch = item.SortOrder <= MaxSortOrder.Value;

            bool createdMatch = true;
            if (MinCreatedAt.HasValue)
                createdMatch = item.CreatedAt >= MinCreatedAt.Value;
            if (createdMatch && MaxCreatedAt.HasValue)
                createdMatch = item.CreatedAt <= MaxCreatedAt.Value;

            if (!titleMatch || !orderMatch || !createdMatch)
                return false;

            if (string.IsNullOrWhiteSpace(FilterText)) return true;

            var f = FilterText.ToLower();
            return item.MethodName.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                   item.TheoryTitle.Contains(f, StringComparison.OrdinalIgnoreCase) ||
                   item.TheoryContent.Contains(f, StringComparison.OrdinalIgnoreCase);
        }
    }
}