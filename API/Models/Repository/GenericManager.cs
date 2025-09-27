using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TD1.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TD1.Attributes;
using TD1.Models;
using TD1.Extensions;

namespace TD1.Repository;

public abstract class GenericManager<T> : IDataRepository<T> where T : class, IEntity
{
    protected readonly AppDbContext _context;

    protected GenericManager(AppDbContext context)
    {
        _context = context;
    }
    public async Task<ActionResult<IEnumerable<T>>> GetAllAsync()
    {
        return await _context.Set<T>().IncludeNavigationPropertiesIfNeeded().ToListAsync();
    }

    public async Task<ActionResult<T>> GetByStringAsync(string str)
    {
        var namePropertyMap = new Dictionary<Type, string>
        {
            { typeof(Marque), nameof(Marque.NomMarque) },
            { typeof(Product), nameof(Product.NomProduit) },
            { typeof(TypeProduit), nameof(TypeProduit.NomTypeProduit) }
        };
        if (!namePropertyMap.TryGetValue(typeof(T), out var propertyName))
        {
            throw new NotSupportedException($"{typeof(T).Name} is not supported");
        }
        return await _context.Set<T>().Where(e => EF.Property<string>(e, propertyName).ToLower() == str.ToLower()).IncludeNavigationPropertiesIfNeeded().FirstOrDefaultAsync();
    }

    public async Task<ActionResult<T>> GetByIdAsync(int id)
    {
        return _context.Set<T>()
            .IncludeNavigationPropertiesIfNeeded()
            .AsEnumerable()          
            .FirstOrDefault(e => e.GetId() == id);
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entityToUpdate, T entity)
    {
        _context.Set<T>().Attach(entityToUpdate);
        _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<ActionResult<IEnumerable<T>>> GetListByProperty(Expression<Func<T, bool>> predicate)
    {
        var query = _context.Set<T>().IncludeNavigationPropertiesIfNeeded(); // ⚡ inclure les navigations
        var result = await query.Where(predicate).ToListAsync();
        return result;
    }
}