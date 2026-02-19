using MaterialDesignThemes.Wpf;

namespace Hairulin_02_01
{
    internal class ThemeHelper
    {
        public static void ToggleTheme()
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