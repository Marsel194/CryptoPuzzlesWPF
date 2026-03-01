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
                theme.SetBaseTheme(BaseTheme.Light);
            else
                theme.SetBaseTheme(BaseTheme.Dark);

            paletteHelper.SetTheme(theme);
        }
    }
}