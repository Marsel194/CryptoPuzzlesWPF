using MaterialDesignThemes.Wpf;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Hairulin_02_01
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow();
            window.Show();
        }
    }

}
