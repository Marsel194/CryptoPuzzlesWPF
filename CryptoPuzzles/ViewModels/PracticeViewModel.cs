using CryptoPuzzles.Helpers;
using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;

namespace CryptoPuzzles.ViewModels
{
    public class PracticeViewModel : ViewModelBase
    {
        private readonly DifficultyApiService _difficultyApi;
        private readonly PuzzleApiService _puzzleApi;
        private readonly HintApiService _hintApi;
        private readonly GameSessionApiService _sessionApi;
        private readonly Action _goBack;
        private readonly int _userId;

        private ObservableCollection<ADifficulty> _difficulties;
        private ADifficulty _selectedDifficulty;
        private ObservableCollection<APuzzle> _puzzles;
        private int _currentPuzzleIndex;
        private bool _isSelectingDifficulty;
        private bool _isSolvingPuzzle;
        private bool _isResult;
        private bool _isModuleCompleted;
        private APuzzle _currentPuzzle;
        private string _elapsedTime;
        private Stopwatch _stopwatch;
        private bool _hasNextPuzzle;
        private string _userAnswer;
        private ObservableCollection<AHint> _hints;
        private int _currentHintIndex;
        private string _currentHint;
        private bool _hasHints;
        private bool _hasNextHint;
        private string _resultIcon;
        private SolidColorBrush _resultColor;
        private string _resultMessage;
        private int _earnedScore;
        private string _completionMessage;

        public PracticeViewModel(
            DifficultyApiService difficultyApi,
            PuzzleApiService puzzleApi,
            HintApiService hintApi,
            GameSessionApiService sessionApi,
            int userId,
            Action goBack)
        {
            _difficultyApi = difficultyApi;
            _puzzleApi = puzzleApi;
            _hintApi = hintApi;
            _sessionApi = sessionApi;
            _userId = userId;
            _goBack = goBack;

            _difficulties = new ObservableCollection<ADifficulty>();
            _puzzles = new ObservableCollection<APuzzle>();
            _hints = new ObservableCollection<AHint>();
            _stopwatch = new Stopwatch();

            SelectDifficultyCommand = new AsyncRelayCommand<ADifficulty>(async d => await SelectDifficultyAsync(d));
            NextHintCommand = new AsyncRelayCommand(async _ => await NextHintAsync(), _ => HasNextHint);
            CheckAnswerCommand = new AsyncRelayCommand(async _ => await CheckAnswerAsync(), _ => !string.IsNullOrWhiteSpace(UserAnswer));
            NextPuzzleCommand = new AsyncRelayCommand(async _ => await NextPuzzleAsync());
            FinishModuleCommand = new AsyncRelayCommand(async _ => await FinishModuleAsync());
            GoBackCommand = new AsyncRelayCommand(async _ => { _goBack(); await Task.CompletedTask; });

            LoadDifficultiesAsync().SafeFireAndForget();
        }

        public ObservableCollection<ADifficulty> Difficulties
        {
            get => _difficulties;
            set => SetProperty(ref _difficulties, value);
        }

        public ADifficulty SelectedDifficulty
        {
            get => _selectedDifficulty;
            set => SetProperty(ref _selectedDifficulty, value);
        }

        public ObservableCollection<APuzzle> Puzzles
        {
            get => _puzzles;
            set => SetProperty(ref _puzzles, value);
        }

        public int CurrentPuzzleIndex
        {
            get => _currentPuzzleIndex;
            set
            {
                if (SetProperty(ref _currentPuzzleIndex, value))
                {
                    UpdateCurrentPuzzle();
                    OnPropertyChanged(nameof(HasNextPuzzle));
                }
            }
        }

        public bool IsSelectingDifficulty
        {
            get => _isSelectingDifficulty;
            set => SetProperty(ref _isSelectingDifficulty, value);
        }

        public bool IsSolvingPuzzle
        {
            get => _isSolvingPuzzle;
            set => SetProperty(ref _isSolvingPuzzle, value);
        }

        public bool IsResult
        {
            get => _isResult;
            set => SetProperty(ref _isResult, value);
        }

        public bool IsModuleCompleted
        {
            get => _isModuleCompleted;
            set => SetProperty(ref _isModuleCompleted, value);
        }

        public APuzzle CurrentPuzzle
        {
            get => _currentPuzzle;
            set => SetProperty(ref _currentPuzzle, value);
        }

        public string ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        public bool HasNextPuzzle => CurrentPuzzleIndex < Puzzles.Count - 1;

        public string UserAnswer
        {
            get => _userAnswer;
            set
            {
                if (SetProperty(ref _userAnswer, value))
                    ((AsyncRelayCommand)CheckAnswerCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<AHint> Hints
        {
            get => _hints;
            set => SetProperty(ref _hints, value);
        }

        public int CurrentHintIndex
        {
            get => _currentHintIndex;
            set
            {
                if (SetProperty(ref _currentHintIndex, value))
                {
                    UpdateCurrentHint();
                    OnPropertyChanged(nameof(HasNextHint));
                }
            }
        }

        public string CurrentHint
        {
            get => _currentHint;
            set => SetProperty(ref _currentHint, value);
        }

        public bool HasHints
        {
            get => _hasHints;
            set => SetProperty(ref _hasHints, value);
        }

        public bool HasNextHint => Hints != null && CurrentHintIndex < Hints.Count - 1;

        public string ResultIcon
        {
            get => _resultIcon;
            set => SetProperty(ref _resultIcon, value);
        }

        public SolidColorBrush ResultColor
        {
            get => _resultColor;
            set => SetProperty(ref _resultColor, value);
        }

        public string ResultMessage
        {
            get => _resultMessage;
            set => SetProperty(ref _resultMessage, value);
        }

        public int EarnedScore
        {
            get => _earnedScore;
            set => SetProperty(ref _earnedScore, value);
        }

        public string CompletionMessage
        {
            get => _completionMessage;
            set => SetProperty(ref _completionMessage, value);
        }

        public ICommand SelectDifficultyCommand { get; }
        public ICommand NextHintCommand { get; }
        public ICommand CheckAnswerCommand { get; }
        public ICommand NextPuzzleCommand { get; }
        public ICommand FinishModuleCommand { get; }
        public ICommand GoBackCommand { get; }

        private async Task LoadDifficultiesAsync()
        {
            try
            {
                var difficulties = await _difficultyApi.GetAllAsync();
                Difficulties = new ObservableCollection<ADifficulty>(difficulties);
                IsSelectingDifficulty = true;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки сложностей: {ex.Message}");
            }
        }

        private async Task SelectDifficultyAsync(ADifficulty difficulty)
        {
            if (difficulty == null) return;
            SelectedDifficulty = difficulty;
            await LoadPuzzlesForDifficultyAsync(difficulty.Id);
        }

        private async Task LoadPuzzlesForDifficultyAsync(int difficultyId)
        {
            try
            {
                var allPuzzles = await _puzzleApi.GetAllAsync();
                // Для практики берём пазлы, которые не являются тренировочными (is_training = false)
                var filtered = allPuzzles.Where(p => p.DifficultyId == difficultyId && !p.IsTraining).ToList();
                // Перемешиваем для случайного порядка
                var rnd = new Random();
                Puzzles = new ObservableCollection<APuzzle>(filtered.OrderBy(x => rnd.Next()));

                if (Puzzles.Any())
                {
                    IsSelectingDifficulty = false;
                    IsSolvingPuzzle = true;
                    CurrentPuzzleIndex = 0;
                }
                else
                {
                    await DialogService.ShowMessage("В этой сложности пока нет головоломок.");
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки пазлов: {ex.Message}");
            }
        }

        private async Task LoadHintsForPuzzleAsync(int puzzleId)
        {
            try
            {
                var allHints = await _hintApi.GetAllAsync();
                var hints = allHints.Where(h => h.PuzzleId == puzzleId).OrderBy(h => h.HintOrder).ToList();
                Hints = new ObservableCollection<AHint>(hints);
                HasHints = Hints.Any();
                CurrentHintIndex = -1;
            }
            catch
            {
                Hints.Clear();
                HasHints = false;
            }
        }

        private void UpdateCurrentPuzzle()
        {
            if (CurrentPuzzleIndex >= 0 && CurrentPuzzleIndex < Puzzles.Count)
            {
                CurrentPuzzle = Puzzles[CurrentPuzzleIndex];
                UserAnswer = string.Empty;
                // Сброс подсказок
                CurrentHintIndex = -1;
                CurrentHint = string.Empty;
                LoadHintsForPuzzleAsync(CurrentPuzzle.Id).SafeFireAndForget();
                // Запуск таймера
                _stopwatch.Restart();
                Task.Run(UpdateTimer);
            }
        }

        private async Task UpdateTimer()
        {
            while (IsSolvingPuzzle && !IsResult && !IsModuleCompleted)
            {
                await Task.Delay(100);
                ElapsedTime = _stopwatch.Elapsed.ToString(@"mm\:ss");
            }
        }

        private void UpdateCurrentHint()
        {
            if (CurrentHintIndex >= 0 && CurrentHintIndex < Hints.Count)
                CurrentHint = Hints[CurrentHintIndex].HintText;
            else
                CurrentHint = string.Empty;
        }

        private async Task NextHintAsync()
        {
            if (HasNextHint)
                CurrentHintIndex++;
        }

        private async Task CheckAnswerAsync()
        {
            if (string.IsNullOrWhiteSpace(UserAnswer) || CurrentPuzzle == null)
                return;

            _stopwatch.Stop();
            bool isCorrect = UserAnswer.Trim().Equals(CurrentPuzzle.Answer.Trim(), StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                // Начисление очков
                EarnedScore = CurrentPuzzle.MaxScore;
                ResultIcon = "CheckCircle";
                ResultColor = Brushes.Green;
                ResultMessage = "Правильно!";
            }
            else
            {
                EarnedScore = 0;
                ResultIcon = "CloseCircle";
                ResultColor = Brushes.Red;
                ResultMessage = "Неправильно!";
            }

            IsSolvingPuzzle = false;
            IsResult = true;
        }

        private async Task NextPuzzleAsync()
        {
            IsResult = false;
            if (CurrentPuzzleIndex < Puzzles.Count - 1)
            {
                CurrentPuzzleIndex++;
                IsSolvingPuzzle = true;
            }
            else
            {
                // Все пазлы решены
                await CompleteModuleAsync();
            }
        }

        private async Task FinishModuleAsync()
        {
            await CompleteModuleAsync();
        }

        private async Task CompleteModuleAsync()
        {
            IsResult = false;
            IsModuleCompleted = true;
            CompletionMessage = $"Вы решили все головоломки сложности \"{SelectedDifficulty.DifficultyName}\"!";
        }
    }
}