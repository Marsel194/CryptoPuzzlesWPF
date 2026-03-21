using CryptoPuzzles.Client.Services;
using CryptoPuzzles.Client.Services.ApiService;
using CryptoPuzzles.Client.ViewModels;
using CryptoPuzzles.Client.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;

namespace CryptoPuzzles.Client
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
            Dispatcher.CurrentDispatcher.UnhandledException += OnDispatcherUnhandledException;

            var services = new ServiceCollection();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5206"),
                Timeout = TimeSpan.FromSeconds(12)
            };

            services.AddSingleton<NavigationService>();
            services.AddSingleton<AdminStatsService>();
            services.AddSingleton<UserSessionService>();
            services.AddSingleton(httpClient);

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
            services.AddSingleton<AdminApiService>();
            services.AddSingleton<AuthApiService>();
            services.AddSingleton<IAuthService, AuthService>();

            Services = services.BuildServiceProvider();
        }

        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            MessageBox.Show($"Ошибка: {ex?.Message}\n{ex?.StackTrace}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show($"Ошибка в задаче: {e.Exception.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            e.SetObserved();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Ошибка UI: {e.Exception.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
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

            mainWindow.Loaded += async (s, args) =>
            {
                if (mainWindow.DataContext is UserViewModel userVm)
                {
                    await userVm.InitializeAsync();
                }
            };

            mainWindow.Show();
        }
    }
}