using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    public class PuzzlesViewModel : EntityViewModelBase<APuzzle, APuzzleCreate, APuzzleUpdate>
    {
        private readonly DifficultyApiService _difficultyApi;
        private readonly EncryptionMethodApiService _methodApi;
        private ObservableCollection<ADifficulty> _difficulties;
        private ObservableCollection<AEncryptionMethod> _methods;

        public PuzzlesViewModel(PuzzleApiService puzzleApi, DifficultyApiService difficultyApi, EncryptionMethodApiService methodApi)
            : base(puzzleApi)
        {
            _difficultyApi = difficultyApi;
            _methodApi = methodApi;
            LoadLookupsAsync();
        }

        public ObservableCollection<ADifficulty> Difficulties { get => _difficulties; set => SetProperty(ref _difficulties, value); }
        public ObservableCollection<AEncryptionMethod> Methods { get => _methods; set => SetProperty(ref _methods, value); }

        private async Task LoadLookupsAsync()
        {
            Difficulties = new ObservableCollection<ADifficulty>(await _difficultyApi.GetAllAsync());
            Methods = new ObservableCollection<AEncryptionMethod>(await _methodApi.GetAllAsync());
        }

        protected override APuzzle CreateNewItem()
        {
            return new APuzzle(0, "", "", "", 50, 0, "", null, null, false, null, null, DateTime.Now);
        }

        protected override APuzzleCreate MapToCreateDto(APuzzle item)
        {
            return new APuzzleCreate(item.Title, item.Content, item.Answer, item.MaxScore,
                item.DifficultyId, item.MethodId, item.IsTraining, item.TutorialOrder);
        }

        protected override APuzzleUpdate MapToUpdateDto(APuzzle item)
        {
            return new APuzzleUpdate(item.Id, item.Title, item.Content, item.Answer, item.MaxScore,
                item.DifficultyId, item.MethodId, item.IsTraining, item.TutorialOrder);
        }

        protected override int GetId(APuzzle item) => item.Id;

        protected override async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(NewItem?.Title))
            {
                DialogService.ShowError("Название не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.Content))
            {
                DialogService.ShowError("Содержание не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.Answer))
            {
                DialogService.ShowError("Ответ не может быть пустым!");
                return;
            }
            if (NewItem.DifficultyId <= 0)
            {
                DialogService.ShowError("Выберите сложность!");
                return;
            }
            // methodId может быть null, это допустимо

            var itemToAdd = new APuzzle(0, NewItem.Title, NewItem.Content, NewItem.Answer,
                NewItem.MaxScore, NewItem.DifficultyId,
                NewItem.MethodId.HasValue ? NewItem.MethodId.Value.ToString() : null, // поле MethodName? В конструкторе APuzzle, возможно, нужно корректно заполнить все поля.
                null, null, NewItem.IsTraining, NewItem.TutorialOrder, null, DateTime.Now);

            // Уточните конструктор APuzzle. Если он не позволяет задать MethodName и т.п., можно создать через копирование свойств.
            // Проще: использовать автоматическое копирование через рефлексию или присвоить поля вручную.
            // Для примера допустим, что у APuzzle есть все сеттеры.

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
                if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Content) || string.IsNullOrWhiteSpace(item.Answer))
                {
                    DialogService.ShowError("Заполните все обязательные поля!");
                    return;
                }
            }
            foreach (var item in Items.Except(_addedItems))
            {
                if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Content) || string.IsNullOrWhiteSpace(item.Answer))
                {
                    DialogService.ShowError("Заполните все обязательные поля!");
                    return;
                }
            }

            await base.SaveAsync();
        }

        protected override bool IsEqual(APuzzle x, APuzzle y)
        {
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
    }
}