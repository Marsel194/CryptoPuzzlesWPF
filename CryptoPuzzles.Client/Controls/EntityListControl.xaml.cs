using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoPuzzles.Client.Controls
{
    public partial class EntityListControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(EntityListControl));

        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register(nameof(BackCommand), typeof(ICommand), typeof(EntityListControl));

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register(nameof(AddCommand), typeof(ICommand), typeof(EntityListControl));

        public static readonly DependencyProperty ClearFilterCommandProperty =
            DependencyProperty.Register(nameof(ClearFilterCommand), typeof(ICommand), typeof(EntityListControl));

        public static readonly DependencyProperty ExportToExcelCommandProperty =
            DependencyProperty.Register(nameof(ExportToExcelCommand), typeof(ICommand), typeof(EntityListControl));

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(EntityListControl));

        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register(nameof(FilterText), typeof(string), typeof(EntityListControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsAddPanelVisibleProperty =
            DependencyProperty.Register(nameof(IsAddPanelVisible), typeof(bool), typeof(EntityListControl), new PropertyMetadata(true));

        public static readonly DependencyProperty AddPanelTitleProperty =
            DependencyProperty.Register(nameof(AddPanelTitle), typeof(string), typeof(EntityListControl), new PropertyMetadata("Новая запись"));

        public static readonly DependencyProperty IsSaveButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsSaveButtonVisible), typeof(bool), typeof(EntityListControl), new PropertyMetadata(true));

        public static readonly DependencyProperty MainContentProperty =
            DependencyProperty.Register(nameof(MainContent), typeof(object), typeof(EntityListControl), new PropertyMetadata(null));

        public static readonly DependencyProperty AddPanelContentProperty =
            DependencyProperty.RegisterAttached(
                "AddPanelContent",
                typeof(object),
                typeof(EntityListControl),
                new PropertyMetadata(null, OnAddPanelContentChanged));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ICommand BackCommand
        {
            get => (ICommand)GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }

        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public ICommand ClearFilterCommand
        {
            get => (ICommand)GetValue(ClearFilterCommandProperty);
            set => SetValue(ClearFilterCommandProperty, value);
        }

        public ICommand ExportToExcelCommand
        {
            get => (ICommand)GetValue(ExportToExcelCommandProperty);
            set => SetValue(ExportToExcelCommandProperty, value);
        }

        public ICommand SaveCommand
        {
            get => (ICommand)GetValue(SaveCommandProperty);
            set => SetValue(SaveCommandProperty, value);
        }

        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }

        public bool IsAddPanelVisible
        {
            get => (bool)GetValue(IsAddPanelVisibleProperty);
            set => SetValue(IsAddPanelVisibleProperty, value);
        }

        public string AddPanelTitle
        {
            get => (string)GetValue(AddPanelTitleProperty);
            set => SetValue(AddPanelTitleProperty, value);
        }

        public bool IsSaveButtonVisible
        {
            get => (bool)GetValue(IsSaveButtonVisibleProperty);
            set => SetValue(IsSaveButtonVisibleProperty, value);
        }

        public object MainContent
        {
            get => GetValue(MainContentProperty);
            set => SetValue(MainContentProperty, value);
        }

        public EntityListControl()
        {
            InitializeComponent();
        }

        public static object GetAddPanelContent(DependencyObject obj) => obj.GetValue(AddPanelContentProperty);
        public static void SetAddPanelContent(DependencyObject obj, object value) => obj.SetValue(AddPanelContentProperty, value);

        private static void OnAddPanelContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EntityListControl control)
                control.UpdateAddPanelContent(e.NewValue);
        }

        private void UpdateAddPanelContent(object content)
        {
            AddPanelContent.Content = content;
        }
    }
}