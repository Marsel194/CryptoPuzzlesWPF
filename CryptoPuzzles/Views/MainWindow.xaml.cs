using System.Windows;
using Hairulin_02_01.ViewModels;

namespace Hairulin_02_01
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
