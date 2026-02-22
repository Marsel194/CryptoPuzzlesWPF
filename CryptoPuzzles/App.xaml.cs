using Hairulin_02_01.Services;
using Hairulin_02_01.ViewModels;
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

            Services = services.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow();
            window.Show();
        }
    }
}
