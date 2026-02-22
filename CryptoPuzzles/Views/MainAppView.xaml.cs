using System.Windows;
using System.Windows.Controls;

namespace Hairulin_02_01
{
    public partial class MainAppView : UserControl
    {
        public MainAppView()
        {
            InitializeComponent();
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeHelper.ToggleTheme();
        }
    }
}
