using Application.Models;

namespace Application.ViewModel;

public interface ICrudViewModel<T> where T : class, IEntity, new()
{
    List<T> Items { get; }
    bool IsLoading {get; set;}
    string ErrorMessage { get; }
    event Action? OnChange;

    Task LoadAsync();
    Task<(bool Success, string? ErrorMessage)> AddAsync(T entity);
    Task<(bool Success, string? ErrorMessage)> UpdateAsync(T entity);
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
}