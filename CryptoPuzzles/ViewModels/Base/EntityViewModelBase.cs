using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels.Base
{
    public abstract class EntityViewModelBase<T, TCreate, TUpdate> : ViewModelBase where T : class
    {
        private readonly NavigationService _navigationService;
        protected readonly IEntityApiService<T, TCreate, TUpdate> _apiService;

        private ObservableCollection<T> _items;
        private T _selectedItem;
        private T _newItem;
        private bool _hasChanges;

        protected List<T> _addedItems = [];
        protected List<T> _removedItems = [];
        protected Dictionary<int, T> _originalItems = [];

        protected EntityViewModelBase(IEntityApiService<T, TCreate, TUpdate> apiService)
        {
            _navigationService = App.Services.GetRequiredService<NavigationService>();
            _apiService = apiService;
            _items = [];
            _newItem = CreateNewItem();

            AddCommand = new AsyncRelayCommand(async _ => await AddAsync());
            SaveCommand = new AsyncRelayCommand(async _ => await SaveAsync(), _ => HasChanges);
            DeleteCommand = new AsyncRelayCommand(async id => await DeleteAsync(id as int?), id => id is int i && i > 0);

            BackCommand = new AsyncRelayCommand(async _ =>
            {
                if (HasChanges)
                {
                    var result = await DialogService.ShowConfirmation(
                        "У вас есть несохранённые изменения. Выйти без сохранения?");
                    if (!result) return;
                }
                await _navigationService.NavigateToAsync<AdminViewModel>();
            });
            _ = LoadDataAsync();
        }

        public ObservableCollection<T> Items { get => _items; set => SetProperty(ref _items, value); }
        public T SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public T NewItem { get => _newItem; set => SetProperty(ref _newItem, value); }
        public bool HasChanges { get => _hasChanges; set => SetProperty(ref _hasChanges, value); }

        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand BackCommand { get; }

        protected abstract T CreateNewItem();
        protected abstract TCreate MapToCreateDto(T item);
        protected abstract TUpdate MapToUpdateDto(T item);
        protected abstract int GetId(T item);

        protected virtual async Task LoadDataAsync()
        {
            try
            {
                var list = await _apiService.GetAllAsync();
                Items = new ObservableCollection<T>(list);
                _originalItems = list.ToDictionary(GetId, x => x);
                _addedItems.Clear();
                _removedItems.Clear();
                NewItem = CreateNewItem();
                HasChanges = false;

                OnPropertyChanged(nameof(Items));
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
                HasChanges = true;
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка добавления: {ex.Message}");
            }
        }

        protected virtual async Task DeleteAsync(int? id)
        {
            if (!id.HasValue)
                return;

            var item = Items.FirstOrDefault(x => GetId(x) == id.Value);
            if (item == null)
                return;

            if (id.Value < 0)
            {
                Items.Remove(item);
                _addedItems.Remove(item);
            }
            else
            {
                Items.Remove(item);
                _removedItems.Add(item);
            }
            HasChanges = true;
            await Task.CompletedTask;
        }

        protected virtual async Task SaveAsync()
        {
            try
            {
                foreach (var item in _removedItems)
                    await _apiService.DeleteAsync(GetId(item));

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
            }
            catch (Exception ex)
            {
                await DialogService.ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }

        protected virtual bool IsEqual(T x, T y) => x?.Equals(y) ?? false;
    }
}