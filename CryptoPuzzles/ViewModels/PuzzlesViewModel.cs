using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    public class PuzzlesViewModel : EntityViewModelBase<APuzzle, APuzzleCreate, APuzzleUpdate>
    {
        private readonly DifficultyApiService _difficultyApi;
        private readonly EncryptionMethodApiService _methodApi;
        private ObservableCollection<ADifficulty> _difficulties = [];
        private ObservableCollection<AEncryptionMethod> _methods = [];

        public PuzzlesViewModel(PuzzleApiService puzzleApi, DifficultyApiService difficultyApi, EncryptionMethodApiService methodApi)
            : base(puzzleApi)
        {
            _difficultyApi = difficultyApi;
            _methodApi = methodApi;
            _ = LoadLookupsAsync();
        }

        public ObservableCollection<ADifficulty> Difficulties
        {
            get => _difficulties;
            set => SetProperty(ref _difficulties, value);
        }

        public ObservableCollection<AEncryptionMethod> Methods
        {
            get => _methods;
            set => SetProperty(ref _methods, value);
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                var difficulties = await _difficultyApi.GetAllAsync();
                Difficulties = new ObservableCollection<ADifficulty>(difficulties);

                var methods = await _methodApi.GetAllAsync();
                Methods = new ObservableCollection<AEncryptionMethod>(methods);
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки справочников: {ex.Message}");
            }
        }

        protected override APuzzle CreateNewItem()
        {
            return new APuzzle(
                id: 0,
                title: string.Empty,
                content: string.Empty,
                answer: string.Empty,
                maxScore: 50,
                difficultyId: 0,
                difficultyName: string.Empty,
                methodId: null,
                methodName: null,
                isTraining: false,
                tutorialOrder: null,
                createdByAdminId: null,
                createdByAdminName: null,
                createdAt: DateTime.Now,
                isDeleted: false,
                deletedAt: null
            );
        }

        protected override APuzzleCreate MapToCreateDto(APuzzle item)
        {
            return new APuzzleCreate(
                Title: item.Title,
                Content: item.Content,
                Answer: item.Answer,
                MaxScore: item.MaxScore,
                DifficultyId: item.DifficultyId,
                MethodId: item.MethodId,
                IsTraining: item.IsTraining,
                TutorialOrder: item.TutorialOrder
            );
        }

        protected override APuzzleUpdate MapToUpdateDto(APuzzle item)
        {
            return new APuzzleUpdate(
                Id: item.Id,
                Title: item.Title,
                Content: item.Content,
                Answer: item.Answer,
                MaxScore: item.MaxScore,
                DifficultyId: item.DifficultyId,
                MethodId: item.MethodId,
                IsTraining: item.IsTraining,
                TutorialOrder: item.TutorialOrder
            );
        }

        protected override int GetId(APuzzle item) => item.Id;

        protected override async Task AddAsync()
        {
            if (NewItem == null) return;

            if (string.IsNullOrWhiteSpace(NewItem.Title))
            {
                await DialogService.ShowError("Название не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem.Content))
            {
                await DialogService.ShowError("Содержание не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem.Answer))
            {
                await DialogService.ShowError("Ответ не может быть пустым!");
                return;
            }
            if (NewItem.DifficultyId <= 0)
            {
                await DialogService.ShowError("Выберите сложность!");
                return;
            }

            string difficultyName = Difficulties
                .FirstOrDefault(d => d.Id == NewItem.DifficultyId)?.DifficultyName ?? string.Empty;

            string? methodName = NewItem.MethodId.HasValue
                ? Methods.FirstOrDefault(m => m.Id == NewItem.MethodId.Value)?.Name
                : null;

            var itemToAdd = new APuzzle(
                id: 0,
                title: NewItem.Title,
                content: NewItem.Content,
                answer: NewItem.Answer,
                maxScore: NewItem.MaxScore,
                difficultyId: NewItem.DifficultyId,
                difficultyName: difficultyName,
                methodId: NewItem.MethodId,
                methodName: methodName,
                isTraining: NewItem.IsTraining,
                tutorialOrder: NewItem.TutorialOrder,
                createdByAdminId: null,
                createdByAdminName: null,
                createdAt: DateTime.Now,
                isDeleted: false,
                deletedAt: null
            );

            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            foreach (var item in _addedItems)
            {
                if (string.IsNullOrWhiteSpace(item.Title) ||
                    string.IsNullOrWhiteSpace(item.Content) ||
                    string.IsNullOrWhiteSpace(item.Answer))
                {
                    await DialogService.ShowError("Заполните все обязательные поля!");
                    return;
                }
            }

            foreach (var item in Items.Except(_addedItems))
            {
                if (string.IsNullOrWhiteSpace(item.Title) ||
                    string.IsNullOrWhiteSpace(item.Content) ||
                    string.IsNullOrWhiteSpace(item.Answer))
                {
                    await DialogService.ShowError("Заполните все обязательные поля!");
                    return;
                }
            }

            await base.SaveAsync();
        }

        protected override bool IsEqual(APuzzle x, APuzzle y)
        {
            if (x == null || y == null) return false;

            return x.Id == y.Id &&
                   x.Title == y.Title &&
                   x.Content == y.Content &&
                   x.Answer == y.Answer &&
                   x.MaxScore == y.MaxScore &&
                   x.DifficultyId == y.DifficultyId &&
                   x.MethodId == y.MethodId &&
                   x.IsTraining == y.IsTraining &&
                   x.TutorialOrder == y.TutorialOrder;
        }

        protected override bool FilterPredicate(APuzzle item)
        {
            if (item == null) return false;
            if (string.IsNullOrWhiteSpace(FilterText)) return true;

            var f = FilterText.ToLower();
            return (item.Title?.ToLower().Contains(f) ?? false) ||
                   (item.Content?.ToLower().Contains(f) ?? false) ||
                   (item.Answer?.ToLower().Contains(f) ?? false);
        }
    }
}