using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.ViewModels;
using CryptoPuzzles.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;

namespace CryptoPuzzles
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
                Timeout = TimeSpan.FromSeconds(20)
            };

            services.AddSingleton<DialogService>();
            services.AddSingleton<NavigationService>();
            services.AddSingleton<AdminStatsService>();

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
            
            services.AddSingleton<UserApiService>();
            services.AddSingleton<DifficultyApiService>();
            services.AddSingleton<EncryptionMethodApiService>();
            services.AddSingleton<PuzzleApiService>();
            services.AddSingleton<HintApiService>();
            services.AddSingleton<GameSessionApiService>();
            services.AddSingleton<TutorialApiService>();

            services.AddSingleton(httpClient);
            services.AddSingleton<AdminApiService>();
            services.AddSingleton<AuthApiService>();

            Services = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainVM = Services.GetRequiredService<MainViewModel>();

            var mainWindow = new MainWindow
            {
                DataContext = mainVM
            };

            mainWindow.Show();
        }
    }
}
