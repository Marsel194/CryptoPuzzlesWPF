using CryptoPuzzles.Client.Services;
using CryptoPuzzles.Client.Services.ApiService;
using CryptoPuzzles.Client.ViewModels;
using CryptoPuzzles.Client.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;

namespace CryptoPuzzles.Client
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            var services = new ServiceCollection();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5206"),
                Timeout = TimeSpan.FromSeconds(12)
            };

            services.AddSingleton<NavigationService>();
            services.AddSingleton<AdminStatsService>();
            services.AddSingleton<UserSessionService>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AdminsViewModel>();
            services.AddTransient<AdminViewModel>();
            services.AddTransient<HintsViewModel>();
            services.AddTransient<MethodsViewModel>();
            services.AddTransient<PuzzlesViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<SessionsViewModel>();
            services.AddTransient<TutorialsViewModel>();
            services.AddTransient<UsersViewModel>();
            services.AddTransient<UserViewModel>();
            services.AddTransient<UserProfileViewModel>();
            services.AddTransient<DifficultiesViewModel>();
            services.AddTransient<SessionProgressViewModel>();
            services.AddTransient<UserStatisticsViewModel>();

            services.AddTransient<TrainingViewModel>();
            services.AddTransient<PracticeViewModel>();

            services.AddSingleton<UserApiService>();
            services.AddSingleton<DifficultyApiService>();
            services.AddSingleton<EncryptionMethodApiService>();
            services.AddSingleton<PuzzleApiService>();
            services.AddSingleton<HintApiService>();
            services.AddSingleton<GameSessionApiService>();
            services.AddSingleton<TutorialApiService>();
            services.AddSingleton<SessionProgressApiService>();
            services.AddSingleton<UserStatisticsApiService>();

            services.AddSingleton(httpClient);
            services.AddSingleton<AdminApiService>();
            services.AddSingleton<AuthApiService>();
            services.AddSingleton<IAuthService, AuthService>();
            Services = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainVM = Services.GetRequiredService<MainViewModel>();

            ThemeHelper.ApplyTheme();

            var mainWindow = new MainWindow
            {
                DataContext = mainVM
            };

            mainWindow.Show();
        }
    }
}
