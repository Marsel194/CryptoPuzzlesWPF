using MaterialDesignThemes.Wpf;

namespace CryptoPuzzles
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
        }
    }
}