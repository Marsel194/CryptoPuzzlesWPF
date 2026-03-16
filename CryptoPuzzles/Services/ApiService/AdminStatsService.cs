namespace CryptoPuzzles.Services.ApiService
{
    public class AdminStatsService
    {
        private readonly UserApiService _userApi;
        private readonly AdminApiService _adminApi;
        private readonly EncryptionMethodApiService _methodApi;
        private readonly PuzzleApiService _puzzleApi;
        private readonly HintApiService _hintApi;
        private readonly GameSessionApiService _sessionApi;
        private readonly TutorialApiService _tutorialApi;
        private readonly DifficultyApiService _difficultyApi;

        public AdminStatsService(
            UserApiService userApi,
            AdminApiService adminApi,
            EncryptionMethodApiService methodApi,
            PuzzleApiService puzzleApi,
            HintApiService hintApi,
            GameSessionApiService sessionApi,
            TutorialApiService tutorialApi,
            DifficultyApiService difficultyApi)
        {
            _userApi = userApi;
            _adminApi = adminApi;
            _methodApi = methodApi;
            _puzzleApi = puzzleApi;
            _hintApi = hintApi;
            _sessionApi = sessionApi;
            _tutorialApi = tutorialApi;
            _difficultyApi = difficultyApi;
        }

        public async Task<AdminStats> LoadStatsAsync()
        {
            var stats = new AdminStats();

            try
            {
                var tasks = new List<Task>
                {
                    Task.Run(async () =>
                    {
                        var users = await _userApi.GetAllAsync().ConfigureAwait(false);
                        var activeUsers = users.Where(u => !u.IsDeleted).ToList();
                        stats.TotalUsers = activeUsers.Count;
                        stats.NewUsersToday = activeUsers.Count(u => u.CreatedAt?.Date == DateTime.Today);
                        stats.ActiveUsers = activeUsers.Count;
                    }),
                    Task.Run(async () =>
                    {
                        var admins = await _adminApi.GetAllAsync().ConfigureAwait(false);
                        stats.TotalAdmins = admins.Count(a => !a.IsDeleted);
                    }),
                    Task.Run(async () =>
                    {
                        var methods = await _methodApi.GetAllAsync().ConfigureAwait(false);
                        stats.TotalMethods = methods.Count(m => !m.IsDeleted);
                    }),
                    Task.Run(async () =>
                    {
                        var puzzles = await _puzzleApi.GetAllAsync().ConfigureAwait(false);
                        var activePuzzles = puzzles.Where(p => !p.IsDeleted).ToList();
                        stats.TotalPuzzles = activePuzzles.Count;
                        stats.ActivePuzzles = activePuzzles.Count;
                    }),
                    Task.Run(async () =>
                    {
                        var hints = await _hintApi.GetAllAsync().ConfigureAwait(false);
                        stats.TotalHints = hints.Count(h => !h.IsDeleted);
                    }),
                    Task.Run(async () =>
                    {
                        var sessions = await _sessionApi.GetAllAsync().ConfigureAwait(false);
                        var activeSessions = sessions.Where(s => !s.IsDeleted).ToList();
                        stats.ActiveSessions = activeSessions.Count(s => !s.IsCompleted);
                        stats.TotalSolved = activeSessions.Count(s => s.IsCompleted);
                        stats.SolvedToday = activeSessions.Count(s => s.CompletedAt?.Date == DateTime.Today);
                        stats.AvgScore = activeSessions.Count != 0 ? activeSessions.Average(s => s.TotalScore) : 0;
                    }),
                    Task.Run(async () =>
                    {
                        var tutorials = await _tutorialApi.GetAllAsync().ConfigureAwait(false);
                        stats.TotalTutorials = tutorials.Count(t => !t.IsDeleted);
                    }),
                    Task.Run(async () =>
                    {
                        var difficulties = await _difficultyApi.GetAllAsync().ConfigureAwait(false);
                        stats.TotalDifficulties = difficulties.Count(d => !d.IsDeleted);
                    })
                };

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                stats.ErrorMessage = ex.Message;
            }

            return stats;
        }
    }

    public class AdminStats
    {
        public int TotalUsers { get; set; }
        public int NewUsersToday { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalMethods { get; set; }
        public int TotalPuzzles { get; set; }
        public int ActivePuzzles { get; set; }
        public int DraftPuzzles { get; set; }
        public int TotalHints { get; set; }
        public int ActiveSessions { get; set; }
        public double AvgScore { get; set; }
        public int TotalTutorials { get; set; }
        public int TotalSolved { get; set; }
        public int SolvedToday { get; set; }
        public int TotalDifficulties { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}