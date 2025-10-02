using Application.Models;
using Application.Services;

namespace Application.ViewModel;

public class CrudViewModel<T> : ICrudViewModel<T> where T : class, IEntity, new()
{
    protected readonly IService<T> _service;
    
    public List<T> Items { get; protected set; } = new List<T>();
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; protected set; }
    public event Action? OnChange;

    public CrudViewModel(IService<T> service)
    {
        _service = service;
    }

    protected void NotifyStateChanged() => OnChange?.Invoke();

    public virtual async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();
        try
        {
            Items = (await _service.GetAllAsync())?.ToList() ?? new List<T>();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }
    public virtual async Task<(bool Success, string? ErrorMessage)> AddAsync(T entity)
    {
        try
        {
            await _service.AddAsync(entity);
            await LoadAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
    public virtual async Task<(bool Success, string? ErrorMessage)> UpdateAsync(T entity)
    {
        try
        {
            await _service.UpdateAsync(entity);
            await LoadAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
    public virtual async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            await LoadAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
    
}