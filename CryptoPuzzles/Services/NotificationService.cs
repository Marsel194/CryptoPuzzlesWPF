using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace CryptoPuzzles.Services
{
    public static class NotificationService
    {
        private static readonly Queue<string> _recentMessages = new();
        private static readonly TimeSpan _duplicateWindow = TimeSpan.FromSeconds(2);
        private static DateTime _lastMessageTime = DateTime.MinValue;

        private static Snackbar FindSnackbar()
        {
            if (Application.Current.MainWindow is not Window mainWindow)
                return null!;
            return FindVisualChild<Snackbar>(mainWindow);
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                    return tChild;
                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null!;
        }

        public static void ShowInfo(string message)
        {
            ShowMessage(message, "MaterialDesignSuccessBrush");
        }

        public static void ShowWarning(string message)
        {
            ShowMessage(message, "MaterialDesignWarningBrush");
        }

        public static void ShowError(string message)
        {
            ShowMessage(message, "MaterialDesignErrorBrush");
        }

        private static void ShowMessage(string message, string brushKey)
        {
            var now = DateTime.Now;
            if (_recentMessages.Contains(message) && (now - _lastMessageTime) < _duplicateWindow)
            {
                ShowGroupedMessage(message, brushKey);
                return;
            }

            while (_recentMessages.Count > 5)
                _recentMessages.Dequeue();

            _recentMessages.Enqueue(message);
            _lastMessageTime = now;

            ShowSnackbar(message, brushKey);
        }

        private static void ShowGroupedMessage(string message, string brushKey)
        {
            var groupedMessage = $"⚠️ Повтор: {message}";
            ShowSnackbar(groupedMessage, brushKey);
        }

        private static void ShowSnackbar(string message, string brushKey)
        {
            var snackbar = FindSnackbar();
            if (snackbar == null) return;

            try
            {
                var background = (SolidColorBrush)Application.Current.FindResource(brushKey);
                var bgColor = background.Color;
                var foreground = new SolidColorBrush(
                    (bgColor.R * 0.299 + bgColor.G * 0.587 + bgColor.B * 0.114) > 186
                        ? Colors.Black
                        : Colors.White);

                // Создаем Grid для лучшего контроля макета
                var grid = new Grid
                {
                    Margin = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Для иконки
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Для текста

                // Иконка
                var icon = new PackIcon
                {
                    Kind = brushKey.Contains("Error") ? PackIconKind.Error :
                           brushKey.Contains("Warning") ? PackIconKind.Warning : PackIconKind.InfoCircle,
                    Foreground = foreground,
                    Width = 24,
                    Height = 24,
                    Margin = new Thickness(0, 0, 12, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Grid.SetColumn(icon, 0);
                Grid.SetRow(icon, 0);
                grid.Children.Add(icon);

                // Текст с правильными параметрами для полного отображения
                var textBlock = new TextBlock
                {
                    Text = message,
                    Foreground = foreground,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    LineHeight = 20,
                    LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                    Padding = new Thickness(0, 2, 0, 2) // Небольшой внутренний отступ для символов
                };

                // Убедимся, что TextBlock не обрезается
                TextOptions.SetTextFormattingMode(textBlock, TextFormattingMode.Display);
                TextOptions.SetTextRenderingMode(textBlock, TextRenderingMode.ClearType);

                Grid.SetColumn(textBlock, 1);
                Grid.SetRow(textBlock, 0);
                grid.Children.Add(textBlock);

                // Контейнер с достаточным пространством
                var border = new Border
                {
                    Background = background,
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16, 14, 16, 14), // Сбалансированные отступы
                    MinHeight = 60,
                    MinWidth = 200,
                    MaxWidth = 600,
                    Child = grid,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Обернем в Viewbox на случай проблем с масштабированием
                var viewbox = new Viewbox
                {
                    Stretch = Stretch.None,
                    StretchDirection = StretchDirection.Both,
                    Child = border,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Очищаем очередь перед отправкой, чтобы избежать наложения
                snackbar.MessageQueue?.Clear();

                // Отправляем с правильными параметрами
                snackbar.MessageQueue?.Enqueue(
                    viewbox,
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds(3)
                );
            }
            catch (Exception ex)
            {
                // Fallback с простым текстом
                System.Diagnostics.Debug.WriteLine($"Ошибка показа уведомления: {ex.Message}");

                var fallbackColor = brushKey.Contains("Error") ? Colors.Red :
                                    brushKey.Contains("Warning") ? Colors.Orange : Colors.Green;

                var textBlock = new TextBlock
                {
                    Text = message,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    Padding = new Thickness(16, 14, 16, 14),
                    LineHeight = 20,
                    LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var border = new Border
                {
                    Background = new SolidColorBrush(fallbackColor),
                    CornerRadius = new CornerRadius(8),
                    MinHeight = 60,
                    Child = textBlock,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                snackbar.MessageQueue?.Clear();
                snackbar.MessageQueue?.Enqueue(border, null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        public static void ClearQueue()
        {
            var snackbar = FindSnackbar();
            snackbar?.MessageQueue?.Clear();
            _recentMessages.Clear();
        }
    }
}