using CryptoPuzzles.Helpers;
using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class TrainingViewModel : ViewModelBase
    {
        private readonly TutorialApiService _tutorialApi;
        private readonly PuzzleApiService _puzzleApi;
        private readonly HintApiService _hintApi;
        private readonly GameSessionApiService _sessionApi;
        private readonly Action _goBack;
        private readonly int _userId;

        private ObservableCollection<ATutorial> _tutorials = [];
        private ObservableCollection<APuzzle> _puzzles = [];
        private int _currentTutorialIndex;
        private int _currentPuzzleIndex;
        private bool _isTheoryMode;
        private bool _isPuzzleMode;
        private bool _isCompleted;
        private string _theoryTitle = string.Empty;
        private string _theoryContent = string.Empty;
        private string _theoryProgress = string.Empty;
        private string _puzzleTitle = string.Empty;
        private string _puzzleContent = string.Empty;
        private string _puzzleProgress = string.Empty;
        private ObservableCollection<AHint> _hints = new();
        private int _currentHintIndex;
        private string _currentHint = string.Empty;
        private bool _hasHints;
        private string _userAnswer = string.Empty;

        public TrainingViewModel(
            TutorialApiService tutorialApi,
            PuzzleApiService puzzleApi,
            HintApiService hintApi,
            GameSessionApiService sessionApi,
            int userId,
            Action goBack)
        {
            _tutorialApi = tutorialApi;
            _puzzleApi = puzzleApi;
            _hintApi = hintApi;
            _sessionApi = sessionApi;
            _userId = userId;
            _goBack = goBack;

            PreviousTheoryCommand = new AsyncRelayCommand(async _ => await PreviousTheoryAsync(), _ => CanGoPrevious);
            NextTheoryCommand = new AsyncRelayCommand(async _ => await NextTheoryAsync(), _ => CanGoNext);
            NextHintCommand = new AsyncRelayCommand(async _ => await NextHintAsync(), _ => HasNextHint);
            CheckAnswerCommand = new AsyncRelayCommand(async _ => await CheckAnswerAsync(), _ => !string.IsNullOrWhiteSpace(UserAnswer));
            GoBackCommand = new AsyncRelayCommand(async _ => { _goBack(); await Task.CompletedTask; });

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
            set => SetProperty(ref _puzzles, value);
        }

        public int CurrentTutorialIndex
        {
            get => _currentTutorialIndex;
            set
            {
                if (SetProperty(ref _currentTutorialIndex, value))
                {
                    UpdateTheoryDisplay();
                    OnPropertyChanged(nameof(CanGoPrevious));
                    OnPropertyChanged(nameof(CanGoNext));
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
            set => SetProperty(ref _isTheoryMode, value);
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

        private async Task LoadDataAsync()
        {
            try
            {
                var tutorials = await _tutorialApi.GetAllAsync();
                var allPuzzles = await _puzzleApi.GetAllAsync();
                var trainingPuzzles = allPuzzles.Where(p => p.IsTraining).OrderBy(p => p.TutorialOrder ?? 0).ToList();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Tutorials = new ObservableCollection<ATutorial>(tutorials.OrderBy(t => t.SortOrder));
                    Puzzles = new ObservableCollection<APuzzle>(trainingPuzzles);

                    if (Tutorials.Any())
                    {
                        IsTheoryMode = true;
                        CurrentTutorialIndex = 0;
                        var firstTutorial = Tutorials[0];
                        TheoryTitle = firstTutorial.TheoryTitle;
                        TheoryContent = firstTutorial.TheoryContent;
                        TheoryProgress = $"1/{Tutorials.Count}";

                        OnPropertyChanged(nameof(CanGoPrevious));
                        OnPropertyChanged(nameof(CanGoNext));

                        ((AsyncRelayCommand)PreviousTheoryCommand).RaiseCanExecuteChanged();
                        ((AsyncRelayCommand)NextTheoryCommand).RaiseCanExecuteChanged();
                    }
                    else if (Puzzles.Any())
                    {
                        IsPuzzleMode = true;
                        CurrentPuzzleIndex = 0;
                    }
                    else
                        IsCompleted = true;
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    DialogService.ShowError($"Ошибка загрузки обучения: {ex.Message}"));
            }
        }

        private void UpdateTheoryDisplay()
        {
            if (CurrentTutorialIndex >= 0 && CurrentTutorialIndex < Tutorials.Count)
            {
                var t = Tutorials[CurrentTutorialIndex];
                TheoryTitle = t.TheoryTitle ?? string.Empty;
                TheoryContent = t.TheoryContent ?? string.Empty;
                TheoryProgress = $"{CurrentTutorialIndex + 1}/{Tutorials.Count}";

                OnPropertyChanged(nameof(TheoryTitle));
                OnPropertyChanged(nameof(TheoryContent));

                OnPropertyChanged(nameof(CanGoPrevious));
                OnPropertyChanged(nameof(CanGoNext));
                ((AsyncRelayCommand)PreviousTheoryCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)NextTheoryCommand).RaiseCanExecuteChanged();
            }
        }

        private async Task PreviousTheoryAsync()
        {
            if (CanGoPrevious)
                CurrentTutorialIndex--;
            await Task.CompletedTask;
        }

        private async Task NextTheoryAsync()
        {
            if (CanGoNext)
                CurrentTutorialIndex++;
            else if (CurrentTutorialIndex == Tutorials.Count - 1)
            {
                IsTheoryMode = false;
                if (Puzzles.Any())
                {
                    IsPuzzleMode = true;
                    CurrentPuzzleIndex = 0;
                }
                else
                    IsCompleted = true;
            }
            await Task.CompletedTask;
        }

        private async Task LoadHintsForPuzzleAsync(int puzzleId)
        {
            try
            {
                var allHints = await _hintApi.GetAllAsync();
                var hints = allHints.Where(h => h.PuzzleId == puzzleId).OrderBy(h => h.HintOrder).ToList();

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

        private void UpdatePuzzleDisplay()
        {
            if (CurrentPuzzleIndex >= 0 && CurrentPuzzleIndex < Puzzles.Count)
            {
                var p = Puzzles[CurrentPuzzleIndex];
                PuzzleTitle = p.Title ?? string.Empty;
                PuzzleContent = p.Content ?? string.Empty;
                PuzzleProgress = $"{CurrentPuzzleIndex + 1}/{Puzzles.Count}";

                OnPropertyChanged(nameof(PuzzleTitle));
                OnPropertyChanged(nameof(PuzzleContent));

                UserAnswer = string.Empty;
                CurrentHintIndex = -1;
                CurrentHint = string.Empty;
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
                CurrentHintIndex++;
            await Task.CompletedTask;
        }

        private async Task CheckAnswerAsync()
        {
            if (string.IsNullOrWhiteSpace(UserAnswer) || CurrentPuzzleIndex < 0 || CurrentPuzzleIndex >= Puzzles.Count)
                return;

            var puzzle = Puzzles[CurrentPuzzleIndex];
            bool isCorrect = UserAnswer.Trim().Equals(puzzle.Answer.Trim(), StringComparison.OrdinalIgnoreCase);

            if (isCorrect)
            {
                await DialogService.ShowMessage("Правильно! +" + puzzle.MaxScore + " очков");

                if (CurrentPuzzleIndex < Puzzles.Count - 1)
                    CurrentPuzzleIndex++;
                else
                {
                    IsPuzzleMode = false;
                    IsCompleted = true;
                }
            }
            else
                await DialogService.ShowError("Неправильный ответ. Попробуйте ещё раз.");
        }
    }
}