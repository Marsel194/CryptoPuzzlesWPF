using CryptoPuzzles.Converters;
using CryptoPuzzles.Helpers;
using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CryptoPuzzles.ViewModels
{
    public class PracticeViewModel : ViewModelBase
    {
        private readonly DifficultyApiService _difficultyApi;
        private readonly PuzzleApiService _puzzleApi;
        private readonly HintApiService _hintApi;
        private readonly GameSessionApiService _sessionApi;
        private readonly SessionProgressApiService _sessionProgressApi;
        private readonly Action _goBack;
        private readonly int _userId;
        private readonly DispatcherTimer _timer;

        private int? _currentSessionId;
        private int _totalScore;
        private int _totalHintsUsed;

        private ObservableCollection<ADifficulty> _difficulties = [];
        private ADifficulty _selectedDifficulty = new(0, string.Empty);
        private ObservableCollection<APuzzle> _puzzles = [];
        private int _currentPuzzleIndex;
        private bool _isSelectingDifficulty;
        private bool _isSolvingPuzzle;
        private bool _isResult;
        private bool _isModuleCompleted;
        private APuzzle _currentPuzzle = new();
        private string _elapsedTime = "00:00";
        private string _userAnswer = string.Empty;
        private ObservableCollection<AHint> _hints = [];
        private int _currentHintIndex;
        private string _currentHint = string.Empty;
        private bool _hasHints;
        private string _resultIcon = "Help";
        private SolidColorBrush _resultColor = Brushes.Transparent;
        private string _resultMessage = string.Empty;
        private int _earnedScore;
        private string _completionMessage = string.Empty;
        private DateTime _startTime;

        private Dictionary<int, ASessionProgress> _progressByPuzzleId = [];

        public PracticeViewModel(
            DifficultyApiService difficultyApi,
            PuzzleApiService puzzleApi,
            HintApiService hintApi,
            GameSessionApiService sessionApi,
            SessionProgressApiService sessionProgressApi,
            int userId,
            Action goBack)
        {
            _difficultyApi = difficultyApi;
            _puzzleApi = puzzleApi;
            _hintApi = hintApi;
            _sessionApi = sessionApi;
            _sessionProgressApi = sessionProgressApi;
            _userId = userId;
            _goBack = goBack;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Tick += (s, e) => UpdateElapsedTime();

            SelectDifficultyCommand = new AsyncRelayCommand<ADifficulty>(async d => await SelectDifficultyAsync(d));
            NextHintCommand = new AsyncRelayCommand(async _ => await NextHintAsync(), _ => HasNextHint);
            CheckAnswerCommand = new AsyncRelayCommand(async _ => await CheckAnswerAsync(), _ => !string.IsNullOrWhiteSpace(UserAnswer));
            NextPuzzleCommand = new AsyncRelayCommand(async _ => await NextPuzzleAsync());
            FinishModuleCommand = new AsyncRelayCommand(async _ => await FinishModuleAsync());
            GoBackCommand = new AsyncRelayCommand(async _ => await GoBackAsync());

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
            set
            {
                if (SetProperty(ref _isSolvingPuzzle, value))
                {
                    if (value)
                    {
                        _startTime = DateTime.Now;
                        _timer.Start();
                    }
                    else
                        _timer.Stop();
                }
            }
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
            set
            {
                _currentPuzzle = value;
                OnPropertyChanged(nameof(CurrentPuzzle));
            }
        }

        public string ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        public bool HasNextPuzzle => _puzzles.Count > 0 && CurrentPuzzleIndex < _puzzles.Count - 1;

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

        public bool HasNextHint => _hints.Count > 0 && CurrentHintIndex < _hints.Count - 1;

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
                await CreateSessionAsync();
                var difficulties = await _difficultyApi.GetAllAsync();
                difficulties = difficulties.Where(d => !d.IsDeleted).ToList();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Difficulties = new ObservableCollection<ADifficulty>(difficulties);
                    IsSelectingDifficulty = true;
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    DialogService.ShowError($"Ошибка загрузки сложностей: {ex.Message}"));
            }
        }

        private async Task CreateSessionAsync()
        {
            try
            {
                var sessionCreate = new AGameSessionCreate(_userId, "practice", 0);
                var session = await _sessionApi.CreateAsync(sessionCreate);
                _currentSessionId = session.Id;
                _totalScore = 0;
                _totalHintsUsed = 0;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Не удалось создать сессию: {ex.Message}");
            }
        }

        private async Task UpdateSessionAsync(int? newScore = null, bool? completed = null)
        {
            if (!_currentSessionId.HasValue) return;

            try
            {
                var update = new AGameSessionUpdate(
                    _currentSessionId.Value,
                    newScore,
                    completed,
                    completed == true ? DateTime.UtcNow : null,
                    CurrentTutorialIndex: null
                );
                await _sessionApi.UpdateAsync(_currentSessionId.Value, update);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Practice] Error updating session: {ex}");
            }
        }

        private async Task UpdateProgressForCurrentPuzzleAsync(bool solved, int hintsUsed, int scoreEarned)
        {
            if (!_currentSessionId.HasValue || CurrentPuzzle == null) return;

            if (_progressByPuzzleId.TryGetValue(CurrentPuzzle.Id, out var progress))
            {
                var update = new ASessionProgressUpdate(
                    progress.Id,
                    hintsUsed,
                    scoreEarned,
                    solved,
                    solved ? DateTime.UtcNow : null
                );
                await _sessionProgressApi.UpdateAsync(progress.Id, update);
            }
        }

        private async Task GoBackAsync()
        {
            if (_currentSessionId.HasValue && !IsModuleCompleted)
            {
                await UpdateSessionAsync(completed: true);
            }

            if (IsSelectingDifficulty)
                _goBack();
            else
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Puzzles.Clear();
                    IsSolvingPuzzle = false;
                    IsResult = false;
                    IsModuleCompleted = false;
                    IsSelectingDifficulty = true;
                    SelectedDifficulty = new ADifficulty(0, string.Empty);
                    Hints.Clear();
                    CurrentHint = string.Empty;
                    ElapsedTime = "00:00";
                    _currentSessionId = null;
                    _totalScore = 0;
                    _totalHintsUsed = 0;
                    _progressByPuzzleId.Clear();
                });
            }
            await Task.CompletedTask;
        }

        private async Task SelectDifficultyAsync(ADifficulty? difficulty)
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
                allPuzzles = allPuzzles.Where(p => !p.IsDeleted).ToList();
                var filtered = allPuzzles.Where(p => p.DifficultyId == difficultyId && !p.IsTraining).ToList();
                var rnd = new Random();
                var shuffled = filtered.OrderBy(x => rnd.Next()).ToList();

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    Puzzles = new ObservableCollection<APuzzle>(shuffled);

                    if (Puzzles.Any())
                    {
                        var existingProgress = await _sessionProgressApi.GetAllAsync(sessionId: _currentSessionId.Value);
                        _progressByPuzzleId = existingProgress.ToDictionary(p => p.PuzzleId, p => p);

                        foreach (var puzzle in Puzzles)
                        {
                            if (!_progressByPuzzleId.ContainsKey(puzzle.Id))
                            {
                                var progressCreate = new ASessionProgressCreate(
                                    _currentSessionId.Value,
                                    puzzle.Id,
                                    Puzzles.IndexOf(puzzle),
                                    0,
                                    0
                                );
                                var newProgress = await _sessionProgressApi.CreateAsync(progressCreate);
                                _progressByPuzzleId[puzzle.Id] = newProgress;
                            }
                        }

                        IsSelectingDifficulty = false;
                        IsResult = false;
                        IsModuleCompleted = false;
                        IsSolvingPuzzle = true;
                        CurrentPuzzleIndex = 0;
                        CurrentPuzzle = Puzzles[0];
                    }
                    else
                        await DialogService.ShowMessage("В этой сложности пока нет головоломок.");
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    DialogService.ShowError($"Ошибка загрузки пазлов: {ex.Message}"));
            }
        }

        private async Task LoadHintsForPuzzleAsync(int puzzleId)
        {
            try
            {
                var allHints = await _hintApi.GetAllAsync();
                var hints = allHints
                    .Where(h => h.PuzzleId == puzzleId && !h.IsDeleted)
                    .OrderBy(h => h.HintOrder)
                    .ToList();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Hints = new ObservableCollection<AHint>(hints);
                    HasHints = Hints.Any();
                    CurrentHintIndex = -1;
                });
            }
            catch
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Hints.Clear();
                    HasHints = false;
                });
            }
        }

        private void UpdateCurrentPuzzle()
        {
            if (CurrentPuzzleIndex >= 0 && CurrentPuzzleIndex < Puzzles.Count)
            {
                CurrentPuzzle = Puzzles[CurrentPuzzleIndex];
                OnPropertyChanged(nameof(CurrentPuzzle));

                UserAnswer = string.Empty;
                CurrentHintIndex = -1;
                CurrentHint = string.Empty;
                _ = LoadHintsForPuzzleAsync(CurrentPuzzle.Id);
                ElapsedTime = "00:00";
            }
        }

        private void UpdateElapsedTime()
        {
            if (IsSolvingPuzzle)
            {
                var elapsed = DateTime.Now - _startTime;
                ElapsedTime = elapsed.ToString(@"mm\:ss");
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
            {
                CurrentHintIndex++;
                _totalHintsUsed++;
                await UpdateProgressForCurrentPuzzleAsync(false, CurrentHintIndex + 1, 0);
            }
            await Task.CompletedTask;
        }

        private async Task CheckAnswerAsync()
        {
            if (string.IsNullOrWhiteSpace(UserAnswer) || CurrentPuzzle == null)
                return;

            IsSolvingPuzzle = false;

            bool isCorrect = UserAnswer.Trim().Equals(CurrentPuzzle.Answer.Trim(), StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                EarnedScore = CurrentPuzzle.MaxScore;
                ResultIcon = "CheckCircle";
                ResultColor = Brushes.Green;
                ResultMessage = "Правильно!";

                _totalScore += CurrentPuzzle.MaxScore;
                await UpdateSessionAsync(newScore: _totalScore);
                await UpdateProgressForCurrentPuzzleAsync(true, CurrentHintIndex + 1, EarnedScore);
            }
            else
            {
                EarnedScore = 0;
                ResultIcon = "CloseCircle";
                ResultColor = Brushes.Red;
                ResultMessage = "Неправильно!";
                await UpdateProgressForCurrentPuzzleAsync(false, CurrentHintIndex + 1, 0);
            }

            IsResult = true;
            await Task.CompletedTask;
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
                await CompleteModuleAsync();
        }

        private async Task FinishModuleAsync()
        {
            if (CurrentPuzzleIndex >= Puzzles.Count - 1)
                await CompleteModuleAsync();
            else
                await NextPuzzleAsync();
        }

        private async Task CompleteModuleAsync()
        {
            IsResult = false;
            IsModuleCompleted = true;
            CompletionMessage = $"Вы решили все головоломки сложности \"{SelectedDifficulty.DifficultyName}\"!\n\n" +
                               $"Всего очков: {_totalScore}\n" +
                               $"Использовано подсказок: {_totalHintsUsed}";

            await UpdateSessionAsync(newScore: _totalScore, completed: true);
        }
    }
}