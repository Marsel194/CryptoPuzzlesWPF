using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public abstract class EntityViewModelBase<T, TCreate, TUpdate> : ViewModelBase
        where T : class
    {
        protected readonly IEntityApiService<T, TCreate, TUpdate> _apiService;

        private ObservableCollection<T> _items;
        private T _selectedItem;
        private T _newItem;
        private bool _hasChanges;

        protected EntityViewModelBase(IEntityApiService<T, TCreate, TUpdate> apiService)
        {
            _apiService = apiService;
            _items = new ObservableCollection<T>();
            _newItem = CreateNewItem();

            AddCommand = new AsyncRelayCommand(async _ => await AddAsync());
            SaveCommand = new AsyncRelayCommand(async _ => await SaveAsync(), _ => HasChanges);
            DeleteCommand = new AsyncRelayCommand(async id => await DeleteAsync(id as int?), id => id is int i && i > 0);

            LoadDataAsync();
        }
        public ObservableCollection<T> Items { get => _items; set => SetProperty(ref _items, value); }
        public T SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public T NewItem { get => _newItem; set => SetProperty(ref _newItem, value); }
        public bool HasChanges { get => _hasChanges; set => SetProperty(ref _hasChanges, value); }

        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

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
                NewItem = CreateNewItem();
                HasChanges = false;
            }
            catch (Exception ex)
            {
                DialogService.ShowError("Ошибка загрузки: " + ex.Message);
            }
        }

        protected virtual async Task AddAsync()
        {
            try
            {
                var dto = MapToCreateDto(NewItem);
                var created = await _apiService.CreateAsync(dto);
                Items.Add(created);
                NewItem = CreateNewItem();
                HasChanges = true;
            }
            catch (Exception ex)
            {
                DialogService.ShowError("Ошибка добавления: " + ex.Message);
            }
        }

        protected virtual async Task SaveAsync()
        {
            try
            {
                foreach (var item in Items)
                {
                    var id = GetId(item);
                    var dto = MapToUpdateDto(item);
                    await _apiService.UpdateAsync(id, dto);
                }
                HasChanges = false;
                await LoadDataAsync();
                DialogService.ShowMessage("Данные сохранены");
            }
            catch (Exception ex)
            {
                DialogService.ShowError("Ошибка сохранения: " + ex.Message);
            }
        }

        protected virtual async Task DeleteAsync(int? id)
        {
            if (!id.HasValue) return;
            if (!DialogService.ShowConfirmation("Удалить запись?")) return;

            try
            {
                await _apiService.DeleteAsync(id.Value);
                var item = Items.FirstOrDefault(x => GetId(x) == id.Value);
                if (item != null) Items.Remove(item);
                HasChanges = true;
            }
            catch (Exception ex)
            {
                DialogService.ShowError("Ошибка удаления: " + ex.Message);
            }
        }
    }
}