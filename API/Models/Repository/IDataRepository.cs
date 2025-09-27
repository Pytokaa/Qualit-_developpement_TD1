using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace TD1.Repository;

public interface IDataRepository<TEntity>// revoir les choses ici, la signature n'est pas bonne
{
    Task<ActionResult<IEnumerable<TEntity>>> GetAllAsync();
    Task<ActionResult<TEntity>> GetByIdAsync(int id);
    Task<ActionResult<TEntity>> GetByStringAsync(string str);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entityToUpdate, TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task<ActionResult<IEnumerable<TEntity>>> GetListByProperty(Expression<Func<TEntity, bool>> predicate);
}