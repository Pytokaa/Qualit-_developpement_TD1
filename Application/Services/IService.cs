using Application.Models;

namespace Application.Services;

public interface IService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity updatedEntity);
    Task DeleteAsync(int id);
    Task<Product?> GetByNameAsync(string name);
}