using Hairulin_02_01.Services;
using Hairulin_02_01.ViewModels;
using Hairulin_02_01.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Hairulin_02_01
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

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
