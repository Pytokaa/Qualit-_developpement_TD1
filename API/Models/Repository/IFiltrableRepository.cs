using Microsoft.AspNetCore.Mvc;

namespace TD1.Repository;

public interface IFiltrableRepository<TEntity> : IDataRepository<TEntity>
{
    Task<ActionResult<IEnumerable<TEntity>>> FilterAsync(string name, string brand, string productType);
}