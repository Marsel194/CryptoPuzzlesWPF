using System.Windows;
using System.Windows.Controls;

namespace Hairulin_02_01.Views
{
    public partial class UserView : UserControl
    {
        public UserView()
        {
            InitializeComponent();
            MessageBox.Show("ТУА");
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeHelper.ToggleTheme();
        }
    }
}
