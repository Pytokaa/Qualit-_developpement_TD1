using Application.Models;
using Application.Models.StateService;
using Application.Services;

namespace Application.ViewModel;

public class BaseEntityViewModel<T> where T: class, IEntity, new()
{
    protected readonly IService<T> _service;
    public IStateService<T> _ItemToAdd;
    public List<T> Items { get; private set; } = new List<T>();
    public T? CurrentEntity { get; set; }
    public bool IsLoading { get; set; }

    public BaseEntityViewModel(IService<T> service, IStateService<T> ItemToAdd)
    {
        _service = service;
        _ItemToAdd = ItemToAdd;
    }

    public async Task LoadAsync()
    {
        Items = (await _service.GetAllAsync())?.ToList() ??  new List<T>();
        CurrentEntity = Items.FirstOrDefault();
        IsLoading = false;
    }

    public async Task AddAsync()
    {
        if (_ItemToAdd.CurrentEntity != null)
        {
            await _service.AddAsync(_ItemToAdd.CurrentEntity);
        }

        _ItemToAdd.CurrentEntity = new T();
    }

    public async Task UpdateAsync()
    {
        if (CurrentEntity != null)
        {
            await _service.UpdateAsync(CurrentEntity);
        }
    }

    public async Task DeleteAsync()
    {
        if (CurrentEntity != null)
        {
            await _service.DeleteAsync((int)CurrentEntity.GetId());
        }
    }

    public async Task OnSelectorChange(T selectedEntity)
    {
        CurrentEntity = selectedEntity;
    }
    public async Task SubmitAddAsync()
    {
        await AddAsync();
        await LoadAsync(); 
    }

    public async Task SubmitUpdateAsync()
    {
        await UpdateAsync();
        await LoadAsync(); 
    }

    public async Task SubmitDeleteAsync()
    {
        await DeleteAsync();
        await LoadAsync(); 
    }
}