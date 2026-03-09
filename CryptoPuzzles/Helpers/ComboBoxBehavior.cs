using System.Windows;
using System.Windows.Controls;

namespace CryptoPuzzles.Helpers
{
    public static class ComboBoxBehavior
    {
        public static readonly DependencyProperty AutoOpenOnEditProperty =
            DependencyProperty.RegisterAttached(
                "AutoOpenOnEdit",
                typeof(bool),
                typeof(ComboBoxBehavior),
                new PropertyMetadata(false, OnAutoOpenOnEditChanged));

        public static void SetAutoOpenOnEdit(ComboBox element, bool value) => element.SetValue(AutoOpenOnEditProperty, value);
        public static bool GetAutoOpenOnEdit(ComboBox element) => (bool)element.GetValue(AutoOpenOnEditProperty);

        private static void OnAutoOpenOnEditChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBox comboBox && (bool)e.NewValue)
            {
                comboBox.Loaded += (s, _) =>
                {
                    comboBox.Focus();
                    comboBox.IsDropDownOpen = true;
                };
            }
        }
    }
}