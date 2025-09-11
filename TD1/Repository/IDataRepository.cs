using Microsoft.AspNetCore.Mvc;

namespace TD1.Repository;

public interface IDataRepository<TEntity>
{
    Task<ActionResult<IEnumerable<TEntity>>> GetAllAsync();
    Task<ActionResult<TEntity>> GetByIdAsync(int id);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entityToUpdate, TEntity entity);
    Task DeleteAsync(TEntity entity);
}