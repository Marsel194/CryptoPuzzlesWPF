using CryptoPuzzles.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CryptoPuzzles.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = App.Services.GetService<MainViewModel>();
        }
    }
}
