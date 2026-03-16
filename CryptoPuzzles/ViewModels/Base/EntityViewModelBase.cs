using ClosedXML.Excel;
using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Windows.Data;
using System.Windows.Input;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.ViewModels.Base
{
    public abstract class EntityViewModelBase<T, TCreate, TUpdate> : ViewModelBase, IEntityViewModel where T : class
    {
        private readonly NavigationService _navigationService;
        protected readonly IEntityApiService<T, TCreate, TUpdate> _apiService;

        private ObservableCollection<T> _items = [];
        private T _selectedItem = null!;
        private T _newItem = null!;
        private ICollectionView? _itemsView;
        private string _filterText = string.Empty;

        protected List<T> _addedItems = [];
        protected Dictionary<int, T> _originalItems = [];

        protected EntityViewModelBase(IEntityApiService<T, TCreate, TUpdate> apiService)
        {
            _navigationService = App.Services.GetRequiredService<NavigationService>();
            _apiService = apiService;
            _newItem = CreateNewItem();
            _selectedItem = CreateNewItem();

            AddCommand = new AsyncRelayCommand(async _ => await AddAsync());
            SaveCommand = new AsyncRelayCommand(async _ => await SaveAsync(), _ => HasChanges);
            BackCommand = new AsyncRelayCommand(async _ =>
            {
                if (HasChanges)
                {
                    var result = await DialogService.ShowConfirmation("У вас есть несохранённые изменения. Выйти без сохранения?");
                    if (!result) return;
                }
                await _navigationService.NavigateToAsync<AdminViewModel>();
            });
            ClearFilterCommand = new AsyncRelayCommand(async _ => await ClearFilterAsync());
            ExportToExcelCommand = new AsyncRelayCommand(async _ => await ExportToExcelAsync());

            Items.CollectionChanged += Items_CollectionChanged;

            _ = LoadDataAsync();
        }

        public ObservableCollection<T> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public ICollectionView? ItemsView
        {
            get => _itemsView;
            set => SetProperty(ref _itemsView, value);
        }

        public T SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public T NewItem
        {
            get => _newItem;
            set => SetProperty(ref _newItem, value);
        }

        public bool HasChanges
        {
            get
            {
                if (_addedItems.Count != 0)
                    return true;

                foreach (var item in Items.Where(x => !_addedItems.Contains(x)))
                {
                    var id = GetId(item);
                    if (_originalItems.TryGetValue(id, out var original))
                    {
                        if (!IsEqual(original, item))
                            return true;
                    }
                }
                return false;
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (SetProperty(ref _filterText, value))
                    ApplyFilter();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand ExportToExcelCommand { get; }

        bool IEntityViewModel.HasChanges
        {
            get => HasChanges;
            set => throw new NotImplementedException();
        }

        protected abstract T CreateNewItem();
        protected abstract TCreate MapToCreateDto(T item);
        protected abstract TUpdate MapToUpdateDto(T item);
        protected abstract int GetId(T item);
        protected abstract bool FilterPredicate(T item);

        protected virtual async Task LoadDataAsync()
        {
            try
            {
                var list = await _apiService.GetAllAsync();
                Items = new ObservableCollection<T>(list);
                _originalItems = list.ToDictionary(GetId, CloneItem);
                _addedItems.Clear();
                NewItem = CreateNewItem();
                OnPropertyChanged(nameof(HasChanges));

                RefreshView();

                foreach (var item in Items)
                    SubscribeItem(item);
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка загрузки: {ex.Message}");
            }
        }

        protected virtual async Task AddAsync()
        {
            try
            {
                var newItem = CreateNewItem();
                Items.Add(newItem);
                _addedItems.Add(newItem);
                OnPropertyChanged(nameof(HasChanges));
                RefreshView();
                SubscribeItem(newItem);
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка добавления: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        protected virtual async Task SaveAsync()
        {
            try
            {
                foreach (var tempItem in _addedItems.ToList())
                {
                    var dto = MapToCreateDto(tempItem);
                    var created = await _apiService.CreateAsync(dto);
                    var index = Items.IndexOf(tempItem);
                    if (index >= 0)
                    {
                        Items[index] = created;
                        _addedItems.Remove(tempItem);
                    }
                }

                foreach (var item in Items.Except(_addedItems))
                {
                    var id = GetId(item);
                    if (_originalItems.TryGetValue(id, out var original))
                    {
                        if (!IsEqual(original, item))
                        {
                            var dto = MapToUpdateDto(item);
                            await _apiService.UpdateAsync(id, dto);
                        }
                    }
                }

                await LoadDataAsync();
                OnPropertyChanged(nameof(HasChanges));
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }

        protected virtual bool IsEqual(T x, T y) => x?.Equals(y) ?? false;

        private void ApplyFilter()
        {
            if (ItemsView == null) return;
            if (string.IsNullOrWhiteSpace(FilterText))
                ItemsView.Filter = null;
            else
                ItemsView.Filter = item => FilterPredicate((T)item);
        }

        private void RefreshView()
        {
            if (Items != null)
            {
                ItemsView = CollectionViewSource.GetDefaultView(Items);
                if (ItemsView != null)
                {
                    ItemsView.SortDescriptions.Clear();
                    ItemsView.SortDescriptions.Add(new SortDescription("IsDeleted", ListSortDirection.Ascending));
                    ItemsView.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
                    ApplyFilter();
                }
            }
        }

        private async Task ClearFilterAsync()
        {
            FilterText = string.Empty;
            await Task.CompletedTask;
        }

        protected virtual IEnumerable<PropertyInfo> GetExportProperties()
        {
            return typeof(T).GetProperties()
                .Where(p => p.CanRead)
                .Where(p => !p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                .Where(p => p.Name != "IsDeleted" && p.Name != "DeletedAt")
                .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime?));
        }

        private async Task ExportToExcelAsync()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx",
                    FileName = $"{GetType().Name.Replace("ViewModel", "")}_export.xlsx"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                var data = ItemsView?.Cast<T>().ToList() ?? Items.ToList();
                if (data.Count == 0)
                {
                    await DialogService.ShowMessage("Нет данных для экспорта.");
                    return;
                }

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Отчет");

                var properties = GetExportProperties().ToList();
                if (properties.Count == 0)
                {
                    await DialogService.ShowMessage("Нет полей для экспорта.");
                    return;
                }

                int currentRow = 1;

                worksheet.Range(currentRow, 1, currentRow, properties.Count).Merge();
                worksheet.Cell(currentRow, 1).Value = "ОТЧЕТ";
                worksheet.Cell(currentRow, 1).Style
                    .Font.SetBold()
                    .Font.SetFontSize(16)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = $"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
                worksheet.Cell(currentRow, 1).Style.Font.SetItalic();
                currentRow++;

                string filterInfo = string.IsNullOrWhiteSpace(FilterText) ? "Все записи" : $"Фильтр: {FilterText}";
                worksheet.Cell(currentRow, 1).Value = filterInfo;
                worksheet.Cell(currentRow, 1).Style.Font.SetItalic();
                currentRow++;

                currentRow++;

                for (int i = 0; i < properties.Count; i++)
                {
                    var prop = properties[i];
                    var exportNameAttr = prop.GetCustomAttribute<ExportNameAttribute>();
                    string headerName = exportNameAttr?.Name ?? prop.Name;
                    worksheet.Cell(currentRow, i + 1).Value = headerName;
                    worksheet.Cell(currentRow, i + 1).Style
                        .Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                }
                currentRow++;

                for (int row = 0; row < data.Count; row++)
                {
                    var item = data[row];
                    for (int col = 0; col < properties.Count; col++)
                    {
                        var prop = properties[col];
                        var value = prop.GetValue(item);
                        worksheet.Cell(row + currentRow, col + 1).Value = value?.ToString() ?? "";
                    }
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(saveFileDialog.FileName);

                await DialogService.ShowMessage($"Экспорт завершён:\n{saveFileDialog.FileName}");
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка экспорта: {ex.Message}");
            }
        }

        protected virtual T CloneItem(T item)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            var json = JsonSerializer.Serialize(item, options);
            return JsonSerializer.Deserialize<T>(json, options)
                ?? throw new InvalidOperationException("Failed to clone item");
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (T item in e.NewItems)
                    SubscribeItem(item);

            if (e.OldItems != null)
                foreach (T item in e.OldItems)
                    UnsubscribeItem(item);

            OnPropertyChanged(nameof(HasChanges));
        }

        private void SubscribeItem(T item)
        {
            if (item is INotifyPropertyChanged notify)
                notify.PropertyChanged += Item_PropertyChanged;
        }

        private void UnsubscribeItem(T item)
        {
            if (item is INotifyPropertyChanged notify)
                notify.PropertyChanged -= Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasChanges));
        }
    }
}