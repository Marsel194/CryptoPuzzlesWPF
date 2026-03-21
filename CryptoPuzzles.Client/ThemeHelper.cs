using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace CryptoPuzzles.Client
{
    internal class ThemeHelper
    {
        public static async Task ToggleTheme()
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            if (theme.GetBaseTheme() == BaseTheme.Dark)
            {
                theme.SetBaseTheme(BaseTheme.Light);
                Properties.Settings.Default.Theme = "Light";
            }
            else
            {
                theme.SetBaseTheme(BaseTheme.Dark);
                Properties.Settings.Default.Theme = "Dark";
            }

            Properties.Settings.Default.Save();
            paletteHelper.SetTheme(theme);

            UpdateMaterialDesignBody(theme.GetBaseTheme() == BaseTheme.Dark);
        }

        public static void ApplyTheme()
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            if (Properties.Settings.Default.Theme == "Dark")
                theme.SetBaseTheme(BaseTheme.Dark);
            else
                theme.SetBaseTheme(BaseTheme.Light);

            paletteHelper.SetTheme(theme);
            UpdateMaterialDesignBody(theme.GetBaseTheme() == BaseTheme.Dark);
        }

        private static void UpdateMaterialDesignBody(bool isDark)
        {
            Color targetColor = isDark
                ? Color.FromRgb(0xDD, 0xDD, 0xDD)   // #FFDDDDDD
                : Colors.Black;

            var brush = new SolidColorBrush(targetColor);
            brush.Freeze();

            Application.Current.Resources["MaterialDesignBody"] = brush;
        }
    }
}