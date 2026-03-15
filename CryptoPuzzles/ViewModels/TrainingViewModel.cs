using CryptoPuzzles.Helpers;
using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CryptoPuzzles.ViewModels
{
    public class TrainingViewModel : ViewModelBase
    {
        private readonly TutorialApiService _tutorialApi;
        private readonly PuzzleApiService _puzzleApi;
        private readonly HintApiService _hintApi;
        private readonly GameSessionApiService _sessionApi;
        private readonly SessionProgressApiService _sessionProgressApi;
        private readonly Action _goBack;
        private readonly int _userId;
        private readonly DispatcherTimer _hintTimer;

        private ObservableCollection<ATutorial> _tutorials = [];
        private ObservableCollection<APuzzle> _puzzles = [];
        private int _currentTutorialIndex = -1;
        private int _currentPuzzleIndex = -1;
        private bool _isTheoryMode;
        private bool _isPuzzleMode;
        private bool _isCompleted;
        private bool _isButtonCanVisible;
        private string _theoryTitle = string.Empty;
        private string _theoryContent = string.Empty;
        private string _theoryProgress = string.Empty;
        private string _puzzleTitle = string.Empty;
        private string _puzzleContent = string.Empty;
        private string _puzzleProgress = string.Empty;
        private ObservableCollection<AHint> _hints = [];
        private int _currentHintIndex = -1;
        private string _currentHint = string.Empty;
        private bool _hasHints;
        private bool _areHintsVisible;
        private string _userAnswer = string.Empty;

        private int? _currentSessionId;
        private int _totalScore;
        private int _totalHintsUsed;

        private ObservableCollection<ASessionProgress> _sessionProgress = [];
        private Dictionary<int, ASessionProgress> _progressByPuzzleId = [];

        public TrainingViewModel(
            TutorialApiService tutorialApi,
            PuzzleApiService puzzleApi,
            HintApiService hintApi,
            GameSessionApiService sessionApi,
            SessionProgressApiService sessionProgressApi,
            int userId,
            Action goBack)
        {
            _tutorialApi = tutorialApi;
            _puzzleApi = puzzleApi;
            _hintApi = hintApi;
            _sessionApi = sessionApi;
            _sessionProgressApi = sessionProgressApi;
            _userId = userId;
            _goBack = goBack;

            _hintTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _hintTimer.Tick += (s, e) => ShowHintsAfterDelay();

            PreviousTheoryCommand = new AsyncRelayCommand(async _ => await PreviousTheoryAsync(), _ => CanGoPrevious);
            NextTheoryCommand = new AsyncRelayCommand(async _ => await NextTheoryAsync(), _ => CanGoNext);
            NextHintCommand = new AsyncRelayCommand(async _ => await NextHintAsync(), _ => HasNextHint);
            CheckAnswerCommand = new AsyncRelayCommand(async _ => await CheckAnswerAsync(), _ => !string.IsNullOrWhiteSpace(UserAnswer));
            GoBackCommand = new AsyncRelayCommand(async _ => await GoBackAsync());
            StartTrainingPracticeCommand = new AsyncRelayCommand(async _ => await StartTrainingPracticeAsync(), _ => Puzzles.Any());

            LoadDataAsync().SafeFireAndForget();
        }

        public ObservableCollection<ATutorial> Tutorials
        {
            get => _tutorials;
            set => SetProperty(ref _tutorials, value);
        }

        public ObservableCollection<APuzzle> Puzzles
        {
            get => _puzzles;
            set
            {
                if (SetProperty(ref _puzzles, value))
                    UpdateButtonVisibility();
            }
        }

        public int CurrentTutorialIndex
        {
            get => _currentTutorialIndex;
            set
            {
                if (SetProperty(ref _currentTutorialIndex, value))
                {
                    UpdateTheoryDisplay();
                    UpdateNavigationCommands();
                }
            }
        }

        public int CurrentPuzzleIndex
        {
            get => _currentPuzzleIndex;
            set
            {
                if (SetProperty(ref _currentPuzzleIndex, value))
                    UpdatePuzzleDisplay();
            }
        }

        public int TotalPuzzles => Puzzles.Count;

        public bool IsTheoryMode
        {
            get => _isTheoryMode;
            set
            {
                if (SetProperty(ref _isTheoryMode, value))
                    UpdateButtonVisibility();
            }
        }

        public bool IsPuzzleMode
        {
            get => _isPuzzleMode;
            set => SetProperty(ref _isPuzzleMode, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }

        public bool IsButtonCanVisible
        {
            get => _isButtonCanVisible;
            set => SetProperty(ref _isButtonCanVisible, value);
        }

        public string TheoryTitle
        {
            get => _theoryTitle;
            set => SetProperty(ref _theoryTitle, value);
        }

        public string TheoryContent
        {
            get => _theoryContent;
            set => SetProperty(ref _theoryContent, value);
        }

        public string TheoryProgress
        {
            get => _theoryProgress;
            set => SetProperty(ref _theoryProgress, value);
        }

        public string PuzzleTitle
        {
            get => _puzzleTitle;
            set => SetProperty(ref _puzzleTitle, value);
        }

        public string PuzzleContent
        {
            get => _puzzleContent;
            set => SetProperty(ref _puzzleContent, value);
        }

        public string PuzzleProgress
        {
            get => _puzzleProgress;
            set => SetProperty(ref _puzzleProgress, value);
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

        public bool AreHintsVisible
        {
            get => _areHintsVisible;
            set => SetProperty(ref _areHintsVisible, value);
        }

        public bool HasNextHint => _hints.Count > 0 && CurrentHintIndex < _hints.Count - 1;

        public string UserAnswer
        {
            get => _userAnswer;
            set
            {
                if (SetProperty(ref _userAnswer, value))
                    ((AsyncRelayCommand)CheckAnswerCommand).RaiseCanExecuteChanged();
            }
        }

        public bool CanGoPrevious => IsTheoryMode && CurrentTutorialIndex > 0;
        public bool CanGoNext => IsTheoryMode && CurrentTutorialIndex < Tutorials.Count - 1;

        public ICommand PreviousTheoryCommand { get; }
        public ICommand NextTheoryCommand { get; }
        public ICommand NextHintCommand { get; }
        public ICommand CheckAnswerCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand StartTrainingPracticeCommand { get; }

        private void ShowHintsAfterDelay()
        {
            _hintTimer.Stop();
            if (HasHints)
                AreHintsVisible = true;
        }

        private void ResetHintTimer()
        {
            _hintTimer.Stop();
            AreHintsVisible = false;
            if (HasHints)
                _hintTimer.Start();
        }

        private void UpdateNavigationCommands()
        {
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
            ((AsyncRelayCommand)PreviousTheoryCommand).RaiseCanExecuteChanged();
            ((AsyncRelayCommand)NextTheoryCommand).RaiseCanExecuteChanged();
            UpdateButtonVisibility();
        }

        private void UpdateButtonVisibility()
        {
            IsButtonCanVisible = IsTheoryMode && !CanGoNext && Puzzles.Any();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var tutorials = await _tutorialApi.GetAllAsync();
                tutorials = tutorials.Where(t => !t.IsDeleted).OrderBy(t => t.SortOrder).ToList();

                var allPuzzles = await _puzzleApi.GetAllAsync();
                allPuzzles = allPuzzles.Where(p => !p.IsDeleted).ToList();
                var trainingPuzzles = allPuzzles
                    .Where(p => p.IsTraining)
                    .OrderBy(p => p.TutorialOrder ?? 0)
                    .ToList();

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    Tutorials = new ObservableCollection<ATutorial>(tutorials);
                    Puzzles = new ObservableCollection<APuzzle>(trainingPuzzles);

                    var sessions = await _sessionApi.GetAllAsync(userId: _userId, sessionType: "training", isCompleted: false);
                    var existingSession = sessions?.OrderByDescending(s => s.SessionStart).FirstOrDefault();

                    if (existingSession != null)
                    {
                        var progressList = await _sessionProgressApi.GetBySessionIdAsync(existingSession.Id);
                        if (progressList != null && progressList.Any())
                        {
                            var solvedPuzzleIds = progressList.Where(p => p.Solved).Select(p => p.PuzzleId).ToHashSet();
                            int firstUnsolvedIndex = -1;
                            for (int i = 0; i < Puzzles.Count; i++)
                                if (!solvedPuzzleIds.Contains(Puzzles[i].Id))
                                {
                                    firstUnsolvedIndex = i;
                                    break;
                                }

                            if (firstUnsolvedIndex >= 0)
                            {
                                _currentSessionId = existingSession.Id;
                                _sessionProgress = new ObservableCollection<ASessionProgress>(progressList);
                                foreach (var p in progressList)
                                    _progressByPuzzleId[p.PuzzleId] = p;

                                _totalScore = progressList.Sum(p => p.ScoreEarned);
                                _totalHintsUsed = progressList.Sum(p => p.HintsUsed);

                                if (existingSession.CurrentTutorialIndex.HasValue && existingSession.CurrentTutorialIndex.Value < Tutorials.Count)
                                {
                                    IsTheoryMode = true;
                                    IsPuzzleMode = false;
                                    CurrentTutorialIndex = existingSession.CurrentTutorialIndex.Value;
                                }
                                else
                                {
                                    IsTheoryMode = false;
                                    IsPuzzleMode = true;
                                    CurrentPuzzleIndex = firstUnsolvedIndex;
                                    if (CurrentPuzzleIndex >= 0)
                                        await LoadHintsForPuzzleAsync(Puzzles[CurrentPuzzleIndex].Id);
                                }
                            }
                            else
                            {
                                await _sessionApi.UpdateAsync(existingSession.Id, new AGameSessionUpdate(
                                    Id: existingSession.Id,
                                    TotalScore: null,
                                    IsCompleted: true,
                                    CompletedAt: DateTime.UtcNow,
                                    CurrentTutorialIndex: null
                                ));
                            }
                        }
                        else
                            await CreateNewSessionAsync();
                    }
                    else
                        await CreateNewSessionAsync();

                    if (IsTheoryMode)
                    {
                        if (Tutorials.Any() && CurrentTutorialIndex == -1)
                            CurrentTutorialIndex = 0;
                        UpdateTheoryDisplay();
                    }
                    else if (IsPuzzleMode && CurrentPuzzleIndex == -1 && Puzzles.Any())
                    {
                        CurrentPuzzleIndex = 0;
                    }
                    else if (!IsTheoryMode && !IsPuzzleMode && !IsCompleted)
                    {
                        IsCompleted = true;
                    }

                    UpdateNavigationCommands();
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    DialogService.ShowError($"Ошибка загрузки обучения: {ex.Message}"));
            }
        }

        private async Task CreateNewSessionAsync()
        {
            try
            {
                var sessionCreate = new AGameSessionCreate(
                    UserId: _userId,
                    SessionType: "training",
                    TotalScore: 0
                );
                var session = await _sessionApi.CreateAsync(sessionCreate);
                _currentSessionId = session.Id;
                _totalScore = 0;
                _totalHintsUsed = 0;
                _sessionProgress.Clear();
                _progressByPuzzleId.Clear();

                for (int i = 0; i < Puzzles.Count; i++)
                {
                    var puzzle = Puzzles[i];
                    var progressCreate = new ASessionProgressCreate(
                        SessionId: _currentSessionId.Value,
                        PuzzleId: puzzle.Id,
                        PuzzleOrder: i,
                        HintsUsed: 0,
                        ScoreEarned: 0
                    );
                    var progress = await _sessionProgressApi.CreateAsync(progressCreate);
                    _sessionProgress.Add(progress);
                    _progressByPuzzleId[puzzle.Id] = progress;
                }

                if (Tutorials.Any())
                {
                    IsTheoryMode = true;
                    IsPuzzleMode = false;
                    CurrentTutorialIndex = 0;
                    await UpdateSessionAsync(tutorialIndex: 0);
                }
                else if (Puzzles.Any())
                {
                    IsTheoryMode = false;
                    IsPuzzleMode = true;
                    CurrentPuzzleIndex = 0;
                }
                else
                    IsCompleted = true;

                UpdateButtonVisibility();
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Не удалось создать сессию: {ex.Message}");
            }
        }

        private async Task UpdateSessionAsync(int? newScore = null, bool? completed = null, int? tutorialIndex = null)
        {
            if (!_currentSessionId.HasValue) return;
            try
            {
                var update = new AGameSessionUpdate(
                    Id: _currentSessionId.Value,
                    TotalScore: newScore,
                    IsCompleted: completed,
                    CompletedAt: completed == true ? DateTime.UtcNow : null,
                    CurrentTutorialIndex: tutorialIndex
                );
                await _sessionApi.UpdateAsync(_currentSessionId.Value, update);
            }
            catch { }
        }

        private void UpdateTheoryDisplay()
        {
            if (CurrentTutorialIndex >= 0 && CurrentTutorialIndex < Tutorials.Count)
            {
                var t = Tutorials[CurrentTutorialIndex];
                TheoryTitle = t.TheoryTitle ?? string.Empty;
                TheoryContent = t.TheoryContent ?? string.Empty;
                TheoryProgress = $"{CurrentTutorialIndex + 1}/{Tutorials.Count}";
            }
        }

        private async Task PreviousTheoryAsync()
        {
            if (CanGoPrevious)
            {
                CurrentTutorialIndex--;
                await UpdateSessionAsync(tutorialIndex: CurrentTutorialIndex);
            }
        }

        private async Task NextTheoryAsync()
        {
            if (CurrentTutorialIndex < Tutorials.Count - 1)
            {
                CurrentTutorialIndex++;
                AreHintsVisible = false;
                await UpdateSessionAsync(tutorialIndex: CurrentTutorialIndex);
            }
            UpdateButtonVisibility();
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
                    ResetHintTimer();

                    ((AsyncRelayCommand)NextHintCommand).RaiseCanExecuteChanged();
                });
            }
            catch (Exception)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Hints.Clear();
                    HasHints = false;
                    AreHintsVisible = false;
                });
            }
        }

        private void UpdatePuzzleDisplay()
        {
            if (CurrentPuzzleIndex >= 0 && CurrentPuzzleIndex < Puzzles.Count)
            {
                var p = Puzzles[CurrentPuzzleIndex];
                PuzzleTitle = p.Title ?? string.Empty;
                PuzzleContent = p.Content ?? string.Empty;
                PuzzleProgress = $"{CurrentPuzzleIndex + 1}/{Puzzles.Count}";
                UserAnswer = string.Empty;
                CurrentHintIndex = -1;
                CurrentHint = string.Empty;
                ((AsyncRelayCommand)NextHintCommand).RaiseCanExecuteChanged();
                _ = LoadHintsForPuzzleAsync(p.Id);
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

                ((AsyncRelayCommand)NextHintCommand).RaiseCanExecuteChanged();
            }
            await Task.CompletedTask;
        }

        private async Task CheckAnswerAsync()
        {
            _hintTimer.Stop();

            if (string.IsNullOrWhiteSpace(UserAnswer) || CurrentPuzzleIndex < 0 || CurrentPuzzleIndex >= Puzzles.Count)
                return;

            var puzzle = Puzzles[CurrentPuzzleIndex];
            bool isCorrect = UserAnswer.Trim().Equals(puzzle.Answer.Trim(), StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                if (_progressByPuzzleId.TryGetValue(puzzle.Id, out var progress))
                {
                    var update = new ASessionProgressUpdate(
                        Id: progress.Id,
                        HintsUsed: _currentHintIndex + 1,
                        ScoreEarned: puzzle.MaxScore,
                        Solved: true,
                        SolvedAt: DateTime.UtcNow
                    );
                    await _sessionProgressApi.UpdateAsync(progress.Id, update);
                    progress.Solved = true;
                    progress.SolvedAt = DateTime.UtcNow;
                    progress.HintsUsed = _currentHintIndex + 1;
                    progress.ScoreEarned = puzzle.MaxScore;
                }

                _totalScore += puzzle.MaxScore;
                await DialogService.ShowMessage($"Правильно! +{puzzle.MaxScore} очков");

                bool completed = false;
                if (CurrentPuzzleIndex < Puzzles.Count - 1)
                    CurrentPuzzleIndex++;
                else
                {
                    IsPuzzleMode = false;
                    IsCompleted = true;
                    completed = true;
                }

                await UpdateSessionAsync(newScore: _totalScore, completed: completed);
            }
            else
                await DialogService.ShowError("Неправильный ответ. Попробуйте ещё раз.");
        }

        private async Task StartTrainingPracticeAsync()
        {
            IsTheoryMode = false;
            IsPuzzleMode = true;
            CurrentPuzzleIndex = 0;
            await UpdateSessionAsync(tutorialIndex: null);
            UpdatePuzzleDisplay();
            UpdateButtonVisibility();
        }

        private async Task UpdateProgressForCurrentPuzzleAsync(bool solved, int hintsUsed, int scoreEarned)
        {
            if (CurrentPuzzleIndex < 0 || CurrentPuzzleIndex >= Puzzles.Count) return;
            var puzzle = Puzzles[CurrentPuzzleIndex];
            if (_progressByPuzzleId.TryGetValue(puzzle.Id, out var progress))
            {
                var update = new ASessionProgressUpdate(
                    Id: progress.Id,
                    HintsUsed: hintsUsed,
                    ScoreEarned: scoreEarned,
                    Solved: solved,
                    SolvedAt: solved ? DateTime.UtcNow : null
                );
                await _sessionProgressApi.UpdateAsync(progress.Id, update);
                progress.HintsUsed = hintsUsed;
                progress.ScoreEarned = scoreEarned;
                progress.Solved = solved;
                if (solved) progress.SolvedAt = DateTime.UtcNow;
            }
            else
            {
                if (_currentSessionId == null) return;

                var progressCreate = new ASessionProgressCreate(
                    SessionId: _currentSessionId.Value,
                    PuzzleId: puzzle.Id,
                    PuzzleOrder: CurrentPuzzleIndex,
                    HintsUsed: hintsUsed,
                    ScoreEarned: scoreEarned
                );
                var newProgress = await _sessionProgressApi.CreateAsync(progressCreate);
                _progressByPuzzleId[puzzle.Id] = newProgress;
                _sessionProgress.Add(newProgress);
            }
        }

        private async Task GoBackAsync()
        {
            _hintTimer.Stop();
            _goBack();
            await Task.CompletedTask;
        }
    }
}