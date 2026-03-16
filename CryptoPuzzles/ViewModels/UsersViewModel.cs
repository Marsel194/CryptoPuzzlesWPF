using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class UsersViewModel : EntityViewModelBase<AUser, AUserCreate, AUserUpdate>
    {
        private string _newPassword = string.Empty;
        private bool _showDeleted = true;
        private string _loginFilter = string.Empty;
        private string _nameFilter = string.Empty;
        private string _emailFilter = string.Empty;
        private DateTime? _minCreatedAt;
        private DateTime? _maxCreatedAt;
        private int? _minTotalSessions;
        private int? _maxTotalSessions;
        private int? _minTotalScore;
        private int? _maxTotalScore;
        private int? _minPuzzlesSolved;
        private int? _maxPuzzlesSolved;

        public UsersViewModel(UserApiService apiService) : base(apiService) { }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public bool ShowDeleted
        {
            get => _showDeleted;
            set
            {
                if (SetProperty(ref _showDeleted, value))
                    ApplyFilter();
            }
        }

        public string LoginFilter
        {
            get => _loginFilter;
            set
            {
                if (SetProperty(ref _loginFilter, value))
                    ApplyFilter();
            }
        }

        public string NameFilter
        {
            get => _nameFilter;
            set
            {
                if (SetProperty(ref _nameFilter, value))
                    ApplyFilter();
            }
        }

        public string EmailFilter
        {
            get => _emailFilter;
            set
            {
                if (SetProperty(ref _emailFilter, value))
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

        public int? MinTotalSessions
        {
            get => _minTotalSessions;
            set
            {
                if (SetProperty(ref _minTotalSessions, value))
                    ApplyFilter();
            }
        }

        public int? MaxTotalSessions
        {
            get => _maxTotalSessions;
            set
            {
                if (SetProperty(ref _maxTotalSessions, value))
                    ApplyFilter();
            }
        }

        public int? MinTotalScore
        {
            get => _minTotalScore;
            set
            {
                if (SetProperty(ref _minTotalScore, value))
                    ApplyFilter();
            }
        }

        public int? MaxTotalScore
        {
            get => _maxTotalScore;
            set
            {
                if (SetProperty(ref _maxTotalScore, value))
                    ApplyFilter();
            }
        }

        public int? MinPuzzlesSolved
        {
            get => _minPuzzlesSolved;
            set
            {
                if (SetProperty(ref _minPuzzlesSolved, value))
                    ApplyFilter();
            }
        }

        public int? MaxPuzzlesSolved
        {
            get => _maxPuzzlesSolved;
            set
            {
                if (SetProperty(ref _maxPuzzlesSolved, value))
                    ApplyFilter();
            }
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
            return new AUserUpdate(item.Id, item.Login, item.Username, item.Email, item.IsDeleted, item.DeletedAt,
                string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword);
        }

        protected override int GetId(AUser item) => item.Id;

        protected override async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(NewItem?.Login))
            {
                await DialogService.ShowError("Логин не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.Username))
            {
                await DialogService.ShowError("Имя пользователя не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewItem?.Email))
            {
                await DialogService.ShowError("Email не может быть пустым!");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                await DialogService.ShowError("Пароль обязателен при создании!");
                return;
            }

            var itemToAdd = new AUser(0, NewItem.Login, NewItem.Username, NewItem.Email, DateTime.Now);
            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);

            NewItem = CreateNewItem();
            NewPassword = string.Empty;
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            foreach (var item in _addedItems)
            {
                if (string.IsNullOrWhiteSpace(item.Login) ||
                    string.IsNullOrWhiteSpace(item.Username) ||
                    string.IsNullOrWhiteSpace(item.Email))
                {
                    await DialogService.ShowError("Все поля должны быть заполнены!");
                    return;
                }
            }

            foreach (var item in Items.Except(_addedItems))
            {
                if (string.IsNullOrWhiteSpace(item.Login) ||
                    string.IsNullOrWhiteSpace(item.Username) ||
                    string.IsNullOrWhiteSpace(item.Email))
                {
                    await DialogService.ShowError("Все поля должны быть заполнены!");
                    return;
                }
            }

            await base.SaveAsync();
            NewPassword = string.Empty;
        }

        protected override bool IsEqual(AUser x, AUser y)
        {
            return x.Id == y.Id &&
                   x.Login == y.Login &&
                   x.Username == y.Username &&
                   x.Email == y.Email &&
                   x.IsDeleted == y.IsDeleted &&
                   x.DeletedAt == y.DeletedAt;
        }

        protected override bool HasAdditionalFilters() =>
            !ShowDeleted || !string.IsNullOrWhiteSpace(LoginFilter) || !string.IsNullOrWhiteSpace(NameFilter) ||
            !string.IsNullOrWhiteSpace(EmailFilter) || MinCreatedAt.HasValue || MaxCreatedAt.HasValue ||
            MinTotalSessions.HasValue || MaxTotalSessions.HasValue || MinTotalScore.HasValue ||
            MaxTotalScore.HasValue || MinPuzzlesSolved.HasValue || MaxPuzzlesSolved.HasValue;

        protected override bool FilterPredicate(AUser item)
        {
            if (!ShowDeleted && item.IsDeleted)
                return false;

            bool loginMatch = string.IsNullOrWhiteSpace(LoginFilter) ||
                              item.Login.Contains(LoginFilter, StringComparison.OrdinalIgnoreCase);
            bool nameMatch = string.IsNullOrWhiteSpace(NameFilter) ||
                             item.Username.Contains(NameFilter, StringComparison.OrdinalIgnoreCase);
            bool emailMatch = string.IsNullOrWhiteSpace(EmailFilter) ||
                              item.Email.Contains(EmailFilter, StringComparison.OrdinalIgnoreCase);

            bool createdMatch = true;
            if (MinCreatedAt.HasValue)
                createdMatch = item.CreatedAt >= MinCreatedAt.Value;
            if (createdMatch && MaxCreatedAt.HasValue)
                createdMatch = item.CreatedAt <= MaxCreatedAt.Value;

            bool sessionsMatch = true;
            if (MinTotalSessions.HasValue)
                sessionsMatch = item.TotalSessions >= MinTotalSessions.Value;
            if (sessionsMatch && MaxTotalSessions.HasValue)
                sessionsMatch = item.TotalSessions <= MaxTotalSessions.Value;

            bool scoreMatch = true;
            if (MinTotalScore.HasValue)
                scoreMatch = item.TotalScore >= MinTotalScore.Value;
            if (scoreMatch && MaxTotalScore.HasValue)
                scoreMatch = item.TotalScore <= MaxTotalScore.Value;

            bool solvedMatch = true;
            if (MinPuzzlesSolved.HasValue)
                solvedMatch = item.PuzzlesSolved >= MinPuzzlesSolved.Value;
            if (solvedMatch && MaxPuzzlesSolved.HasValue)
                solvedMatch = item.PuzzlesSolved <= MaxPuzzlesSolved.Value;

            return loginMatch && nameMatch && emailMatch && createdMatch && sessionsMatch && scoreMatch && solvedMatch;
        }
    }
}