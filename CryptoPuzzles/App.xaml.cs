using CryptoPuzzles.Services;
using CryptoPuzzles.ViewModels;
using CryptoPuzzles.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CryptoPuzzles
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ApiService>();
            services.AddSingleton<DialogService>();
            services.AddSingleton<NavigationService>();

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

            Services = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainVM = new MainViewModel();
            var mainWindow = new MainWindow
            {
                DataContext = mainVM
            };

            mainWindow.Show();
        }
    }
}
