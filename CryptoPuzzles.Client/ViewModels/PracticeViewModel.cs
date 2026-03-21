// PracticeViewModel.cs
using CryptoPuzzles.Client.Helpers;
using CryptoPuzzles.Client.Services;
using CryptoPuzzles.Client.Services.ApiService;
using CryptoPuzzles.Client.ViewModels.Base;
using CryptoPuzzles.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CryptoPuzzles.Client.ViewModels
{
    public class PracticeViewModel : ViewModelBase, IDisposable
    {
        private readonly PuzzleApiService _puzzleApi;
        private readonly HintApiService _hintApi;
        private readonly GameSessionApiService _sessionApi;
        private readonly SessionProgressApiService _progressApi;
        private readonly DifficultyApiService _difficultyApi;
        private readonly UserSessionService _userSession;
        private readonly Action _goBack;
        private readonly int _userId;
        private readonly DispatcherTimer _timer;
        private readonly DispatcherTimer _hintTimer;
        private readonly HashSet<int> _seenPuzzleIds;
        private bool _isSelectingInProgress;
        private static readonly Random _random = new();

        private ObservableCollection<ADifficulty>? _difficulties;
        private ADifficulty? _selectedDifficulty;
        private ObservableCollection<APuzzle>? _puzzles;
        private int _currentPuzzleIndex = -1;
        private APuzzle? _currentPuzzle;
        private ObservableCollection<AHint>? _hints;
        private int _currentHintIndex = -1;
        private int _hintsUsed = 0;
        private string _currentHint = string.Empty;
        private bool _hasHints;
        private bool _areHintsVisible;
        private string _userAnswer = string.Empty;
        private int? _currentSessionId;
        private int _totalScore;
        private int _totalHintsUsed;
        private DateTime _puzzleStartTime;
        private string _elapsedTime = "00:00";
        private readonly Dictionary<int, ASessionProgress> _progressByPuzzleId = [];
        private HashSet<int> _solvedPuzzleIds = [];

        private bool _isSelectingDifficulty = true;
        private bool _isSolvingPuzzle;
        private bool _isResult;
        private bool _isModuleCompleted;
        private string _resultIcon = string.Empty;
        private string _resultMessage = string.Empty;
        private int _earnedScore;
        private string _completionMessage = string.Empty;

        public PracticeViewModel(
            PuzzleApiService puzzleApi,
            HintApiService hintApi,
            GameSessionApiService sessionApi,
            SessionProgressApiService progressApi,
            DifficultyApiService difficultyApi,
            UserSessionService userSession,
            int userId,
            Action goBack)
        {
            _puzzleApi = puzzleApi;
            _hintApi = hintApi;
            _sessionApi = sessionApi;
            _progressApi = progressApi;
            _difficultyApi = difficultyApi;
            _userSession = userSession;
            _userId = userId;
            _goBack = goBack;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;

            _hintTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _hintTimer.Tick += HintTimer_Tick;

            _seenPuzzleIds = [];

            SelectDifficultyCommand = new AsyncRelayCommand(async _ => await SelectDifficultyAsync(_ as ADifficulty), _ => !_isSelectingInProgress);
            NextHintCommand = new AsyncRelayCommand(async _ => await NextHintAsync(), _ => HasNextHint);
            CheckAnswerCommand = new AsyncRelayCommand(async _ => await CheckAnswerAsync(), _ => !string.IsNullOrWhiteSpace(UserAnswer));
            GoBackCommand = new AsyncRelayCommand(async _ => await GoBackAsync());
            FinishModuleCommand = new AsyncRelayCommand(async _ => await FinishModuleAsync());
            NextRandomQuestionCommand = new AsyncRelayCommand(async _ => await LoadRandomPuzzleAsync());

            LoadDifficultiesAsync().SafeFireAndForget();
        }

        public ObservableCollection<ADifficulty>? Difficulties
        {
            get => _difficulties;
            set => SetProperty(ref _difficulties, value);
        }

        public ADifficulty? SelectedDifficulty
        {
            get => _selectedDifficulty;
            set => SetProperty(ref _selectedDifficulty, value);
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

        public string ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        public APuzzle? CurrentPuzzle
        {
            get => _currentPuzzle;
            set => SetProperty(ref _currentPuzzle, value);
        }

        public bool AreHintsVisible
        {
            get => _areHintsVisible;
            set => SetProperty(ref _areHintsVisible, value);
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

        public bool HasNextHint => _hints?.Count > 0 && _currentHintIndex < _hints.Count - 1;

        public string UserAnswer
        {
            get => _userAnswer;
            set
            {
                if (SetProperty(ref _userAnswer, value))
                    AsyncRelayCommand.RaiseCanExecuteChanged();
            }
        }

        public string ResultIcon
        {
            get => _resultIcon;
            set => SetProperty(ref _resultIcon, value);
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
        public ICommand GoBackCommand { get; }
        public ICommand FinishModuleCommand { get; }
        public ICommand NextRandomQuestionCommand { get; }

        private async Task LoadDifficultiesAsync()
        {
            try
            {
                var difficulties = await _difficultyApi.GetAllAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Difficulties = new ObservableCollection<ADifficulty>(difficulties.Where(d => !d.IsDeleted));
                });
            }
            catch (Exception ex) when (ex.Message.Contains("401"))
            {
                _userSession.ClearUser();
                var navigation = App.Services.GetRequiredService<NavigationService>();
                await navigation.NavigateToAsync<LoginViewModel>();
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки сложностей: {ex.Message}");
            }
        }

        private async Task LoadSolvedPuzzlesAsync()
        {
            try
            {
                var solvedProgress = await _progressApi.GetAllAsync(userId: _userId, solved: true);
                _solvedPuzzleIds = new HashSet<int>(solvedProgress.Select(p => p.PuzzleId));
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки решённых головоломок: {ex.Message}");
                _solvedPuzzleIds = [];
            }
        }

        private async Task SelectDifficultyAsync(ADifficulty? difficulty)
        {
            if (_isSelectingInProgress) return;

            _isSelectingInProgress = true;
            AsyncRelayCommand.RaiseCanExecuteChanged();

            try
            {
                if (difficulty == null) return;

                SelectedDifficulty = difficulty;
                IsSelectingDifficulty = false;
                IsSolvingPuzzle = true;

                await LoadSolvedPuzzlesAsync();
                await LoadPuzzlesForDifficultyAsync(difficulty.Id);

                if (_puzzles == null || _puzzles.Count == 0)
                {
                    await DialogService.ShowError("Нет заданий для выбранной сложности.");
                    GoBackToDifficultySelection();
                    return;
                }

                var unsolvedPuzzles = _puzzles.Where(p => !_solvedPuzzleIds.Contains(p.Id)).ToList();
                if (unsolvedPuzzles.Count == 0)
                {
                    var result = await DialogService.ShowConfirmation("Вы уже решили все головоломки этой сложности. Хотите пройти модуль заново?");
                    if (result)
                    {
                        unsolvedPuzzles = _puzzles.ToList();
                        _puzzles = new ObservableCollection<APuzzle>(unsolvedPuzzles);
                    }
                    else
                    {
                        GoBackToDifficultySelection();
                        return;
                    }
                }
                else
                {
                    _puzzles = new ObservableCollection<APuzzle>(unsolvedPuzzles);
                }

                _currentPuzzleIndex = -1;
                _seenPuzzleIds.Clear();

                await CreateNewSessionAsync();
            }
            finally
            {
                _isSelectingInProgress = false;
                AsyncRelayCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task LoadPuzzlesForDifficultyAsync(int difficultyId)
        {
            try
            {
                var allPuzzles = await _puzzleApi.GetAllAsync();
                var puzzles = allPuzzles
                    .Where(p => !p.IsDeleted && p.DifficultyId == difficultyId && !p.IsTraining)
                    .OrderBy(p => p.Id)
                    .ToList();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _puzzles = new ObservableCollection<APuzzle>(puzzles);
                });
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки заданий: {ex.Message}");
                GoBackToDifficultySelection();
            }
        }

        private async Task CreateNewSessionAsync()
        {
            try
            {
                var sessionCreate = new AGameSessionCreate(
                    UserId: _userId,
                    SessionType: "practice",
                    TotalScore: 0
                );
                var session = await _sessionApi.CreateAsync(sessionCreate);

                if (session == null || session.Id == 0)
                    throw new Exception("Не удалось создать сессию: сервер вернул некорректный ответ.");

                _currentSessionId = session.Id;
                _totalScore = 0;
                _totalHintsUsed = 0;
                _progressByPuzzleId.Clear();

                if (_puzzles != null)
                {
                    for (int i = 0; i < _puzzles.Count; i++)
                    {
                        var puzzle = _puzzles[i];
                        var progressCreate = new ASessionProgressCreate(
                            SessionId: _currentSessionId.Value,
                            PuzzleId: puzzle.Id,
                            PuzzleOrder: i,
                            HintsUsed: 0,
                            ScoreEarned: 0
                        );

                        ASessionProgress? progress = null;
                        int retryCount = 0;
                        while (progress == null && retryCount < 3)
                        {
                            try
                            {
                                progress = await _progressApi.CreateAsync(progressCreate);
                            }
                            catch
                            {
                                retryCount++;
                                if (retryCount >= 3) throw;
                                await Task.Delay(200);
                            }
                        }

                        if (progress != null)
                            _progressByPuzzleId[puzzle.Id] = progress;
                        else
                            throw new Exception($"Не удалось создать прогресс для пазла {puzzle.Id}");
                    }
                }

                MoveToNextPuzzle();
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Не удалось начать практику: {ex.Message}");
                GoBackToDifficultySelection();
            }
        }

        private void MoveToNextPuzzle()
        {
            if (_puzzles == null || _puzzles.Count == 0)
            {
                CompleteModule();
                return;
            }

            _currentPuzzleIndex++;
            if (_currentPuzzleIndex >= _puzzles.Count)
            {
                CompleteModule();
                return;
            }

            var nextPuzzle = _puzzles[_currentPuzzleIndex];
            SetCurrentPuzzle(nextPuzzle);
        }

        private void SetCurrentPuzzle(APuzzle puzzle)
        {
            CurrentPuzzle = puzzle;
            UserAnswer = string.Empty;
            _puzzleStartTime = DateTime.Now;
            _timer.Start();

            if (!_seenPuzzleIds.Contains(puzzle.Id))
                _seenPuzzleIds.Add(puzzle.Id);

            LoadHintsForCurrentPuzzleAsync().SafeFireAndForget();
        }

        private async Task LoadHintsForCurrentPuzzleAsync()
        {
            if (CurrentPuzzle == null) return;

            try
            {
                var allHints = await _hintApi.GetAllAsync();
                var hints = allHints
                    .Where(h => h.PuzzleId == CurrentPuzzle.Id && !h.IsDeleted)
                    .OrderBy(h => h.HintOrder)
                    .ToList();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _hints = new ObservableCollection<AHint>(hints);
                    HasHints = _hints.Any();
                    RestoreHintStateForCurrentPuzzle();
                    ResetHintTimer();
                    AsyncRelayCommand.RaiseCanExecuteChanged();
                });
            }
            catch
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _hints?.Clear();
                    HasHints = false;
                    AreHintsVisible = false;
                    _currentHintIndex = -1;
                    _hintsUsed = 0;
                    CurrentHint = string.Empty;
                });
            }
        }

        private void RestoreHintStateForCurrentPuzzle()
        {
            if (CurrentPuzzle == null || _hints == null) return;

            if (_progressByPuzzleId.TryGetValue(CurrentPuzzle.Id, out var progress) && progress.HintsUsed > 0)
            {
                _hintsUsed = progress.HintsUsed;
                int lastUsedIndex = _hintsUsed - 1;
                _currentHintIndex = lastUsedIndex < _hints.Count ? lastUsedIndex : _hints.Count - 1;
                AreHintsVisible = true;
                UpdateCurrentHint();
                _hintTimer.Stop();
            }
            else
            {
                _hintsUsed = 0;
                _currentHintIndex = -1;
                AreHintsVisible = false;
                CurrentHint = string.Empty;
                if (HasHints)
                {
                    _hintTimer.Start();
                }
            }
        }

        private void UpdateCurrentHint()
        {
            if (_hints != null && _currentHintIndex >= 0 && _currentHintIndex < _hints.Count)
                CurrentHint = _hints[_currentHintIndex].HintText;
            else
                CurrentHint = string.Empty;
        }

        private void ResetHintTimer()
        {
            _hintTimer.Stop();
            if (HasHints && _hintsUsed == 0 && _currentHintIndex == -1)
                _hintTimer.Start();
        }

        private void ShowHintsAfterDelay()
        {
            _hintTimer.Stop();
            if (!HasHints) return;
            if (_hintsUsed == 0 && _currentHintIndex == -1 && _hints != null && _hints.Count > 0)
            {
                _currentHintIndex = 0;
                _hintsUsed = 1;
                UpdateCurrentHint();
                AreHintsVisible = true;
                _ = UpdateProgressForCurrentPuzzleAsync(solved: false, hintsUsed: _hintsUsed, scoreEarned: 0);
                AsyncRelayCommand.RaiseCanExecuteChanged();
            }
        }

        private void UpdateElapsedTime()
        {
            var elapsed = DateTime.Now - _puzzleStartTime;
            ElapsedTime = elapsed.ToString(@"mm\:ss");
        }

        private async Task NextHintAsync()
        {
            if (HasNextHint && _hints != null && CurrentPuzzle != null)
            {
                _currentHintIndex++;
                _hintsUsed++;
                UpdateCurrentHint();
                await UpdateProgressForCurrentPuzzleAsync(solved: false, hintsUsed: _hintsUsed, scoreEarned: 0);
                AsyncRelayCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task CheckAnswerAsync()
        {
            _hintTimer.Stop();
            _timer.Stop();

            if (string.IsNullOrWhiteSpace(UserAnswer) || CurrentPuzzle == null)
                return;

            bool isCorrect = UserAnswer.Trim().Equals(CurrentPuzzle.Answer.Trim(), StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                int score = CalculateScore();
                EarnedScore = score;
                _totalScore += score;

                await UpdateProgressForCurrentPuzzleAsync(solved: true, hintsUsed: _hintsUsed, scoreEarned: score);

                if (_currentSessionId.HasValue)
                {
                    await _sessionApi.UpdateAsync(_currentSessionId.Value, new AGameSessionUpdate(
                        Id: _currentSessionId.Value,
                        TotalScore: _totalScore,
                        IsCompleted: null,
                        CompletedAt: null,
                        CurrentTutorialIndex: null,
                        IsDeleted: null,
                        DeletedAt: null
                    ));
                }

                ResultIcon = "CheckCircle";
                ResultMessage = "Правильно!";
                IsResult = true;
                IsSolvingPuzzle = false;
            }
            else
            {
                await DialogService.ShowError("Неправильный ответ. Попробуйте ещё раз.");
                _timer.Start();
                if (HasHints && !AreHintsVisible && _hintsUsed == 0)
                    _hintTimer.Start();
            }
        }

        private int CalculateScore()
        {
            if (CurrentPuzzle == null) return 0;

            int baseScore = CurrentPuzzle.MaxScore;
            double multiplier = Math.Max(0, 1.0 - (_hintsUsed * 0.2));
            return (int)(baseScore * multiplier);
        }

        private async Task UpdateProgressForCurrentPuzzleAsync(bool solved, int hintsUsed, int scoreEarned)
        {
            if (CurrentPuzzle == null) return;

            if (_progressByPuzzleId.TryGetValue(CurrentPuzzle.Id, out var progress))
            {
                var update = new ASessionProgressUpdate(
                    Id: progress.Id,
                    HintsUsed: hintsUsed,
                    ScoreEarned: scoreEarned,
                    Solved: solved,
                    SolvedAt: solved ? DateTime.UtcNow : null,
                    IsDeleted: null,
                    DeletedAt: null
                );
                await _progressApi.UpdateAsync(progress.Id, update);
                progress.HintsUsed = hintsUsed;
                progress.ScoreEarned = scoreEarned;
                progress.Solved = solved;
                if (solved) progress.SolvedAt = DateTime.UtcNow;
            }
        }

        private async Task FinishModuleAsync()
        {
            IsResult = false;
            IsSolvingPuzzle = false;
            IsModuleCompleted = true;
            CompletionMessage = $"Вы завершили модуль. Набрано очков: {_totalScore}";
            await UpdateSessionCompletedAsync();

            var statsApi = App.Services.GetRequiredService<UserStatisticsApiService>();
            await statsApi.RefreshStatisticsAsync(_userId);
        }

        private async Task UpdateSessionCompletedAsync()
        {
            if (!_currentSessionId.HasValue) return;
            await _sessionApi.UpdateAsync(_currentSessionId.Value, new AGameSessionUpdate(
                Id: _currentSessionId.Value,
                TotalScore: null,
                IsCompleted: true,
                CompletedAt: DateTime.UtcNow,
                CurrentTutorialIndex: null,
                IsDeleted: null,
                DeletedAt: null
            ));
        }

        private void CompleteModule()
        {
            _timer.Stop();
            _hintTimer.Stop();
            IsSolvingPuzzle = false;
            IsModuleCompleted = true;
            CompletionMessage = $"Вы решили все задания сложности {SelectedDifficulty?.DifficultyName ?? "неизвестно"}!\nНабрано очков: {_totalScore}";
        }

        private async Task LoadRandomPuzzleAsync()
        {
            var randomPuzzle = GetRandomPuzzle();
            if (randomPuzzle == null)
            {
                CompleteModule();
                return;
            }

            _timer.Stop();
            _hintTimer.Stop();
            if (_puzzles == null) return;
            _currentPuzzleIndex = _puzzles.IndexOf(randomPuzzle);

            IsResult = false;
            IsSolvingPuzzle = true;

            SetCurrentPuzzle(randomPuzzle);
            await Task.CompletedTask;
        }

        private APuzzle? GetRandomPuzzle()
        {
            if (_puzzles == null || _puzzles.Count == 0)
                return null;

            var unsolvedIds = _puzzles
                .Where(p => !_progressByPuzzleId.TryGetValue(p.Id, out var prog) || !prog.Solved)
                .Select(p => p.Id)
                .ToList();

            if (unsolvedIds.Count == 0)
                return null;

            int index;
            lock (_random)
            {
                index = _random.Next(unsolvedIds.Count);
            }
            int puzzleId = unsolvedIds[index];
            return _puzzles.First(p => p.Id == puzzleId);
        }

        private async Task GoBackAsync()
        {
            _timer.Stop();
            _hintTimer.Stop();
            ResetState();
            _goBack();
            await Task.CompletedTask;
        }

        private void ResetState()
        {
            IsSelectingDifficulty = true;
            IsSolvingPuzzle = false;
            IsResult = false;
            IsModuleCompleted = false;
            SelectedDifficulty = null;
            CurrentPuzzle = null;
            _currentPuzzleIndex = -1;
            _puzzles = null;
            _hints = null;
            _currentHintIndex = -1;
            _hintsUsed = 0;
            CurrentHint = string.Empty;
            UserAnswer = string.Empty;
            ElapsedTime = "00:00";
            _progressByPuzzleId.Clear();
            _seenPuzzleIds.Clear();
            _solvedPuzzleIds.Clear();
        }

        private void GoBackToDifficultySelection()
        {
            IsSelectingDifficulty = true;
            IsSolvingPuzzle = false;
            IsResult = false;
            IsModuleCompleted = false;
        }

        public void Dispose()
        {
            _timer.Stop();
            _hintTimer.Stop();
            _timer.Tick -= Timer_Tick;
            _hintTimer.Tick -= HintTimer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e) => UpdateElapsedTime();

        private void HintTimer_Tick(object? sender, EventArgs e) => ShowHintsAfterDelay();
    }
}