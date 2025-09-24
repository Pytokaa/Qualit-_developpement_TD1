using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TD1.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TD1.Attributes;
using TD1.Models;
using TD1.Extensions;

namespace TD1.Repository;

public class GenericManager<T>(ProduitDbContext context) : IDataRepository<T> where T : class, IEntity
{
    public async Task<ActionResult<IEnumerable<T>>> GetAllAsync()
    {
        return await context.Set<T>().IncludeNavigationPropertiesIfNeeded().ToListAsync();
    }

    public async Task<ActionResult<T>> GetByStringAsync(string str)
    {
        var namePropertyMap = new Dictionary<Type, string>
        {
            { typeof(Marque), nameof(Marque.NomMarque) },
            { typeof(Produit), nameof(Produit.NomProduit) },
            { typeof(TypeProduit), nameof(TypeProduit.NomTypeProduit) }
        };
        if (!namePropertyMap.TryGetValue(typeof(T), out var propertyName))
        {
            throw new NotSupportedException($"{typeof(T).Name} is not supported");
        }
        return await context.Set<T>().Where(e => EF.Property<string>(e, propertyName).ToLower() == str.ToLower()).IncludeNavigationPropertiesIfNeeded().FirstOrDefaultAsync();
    }

    public async Task<ActionResult<T>> GetByIdAsync(int id)
    {
        return context.Set<T>()
            .IncludeNavigationPropertiesIfNeeded()
            .AsEnumerable()          // <-- switch du côté client
            .FirstOrDefault(e => e.GetId() == id);

    }

    public async Task AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entityToUpdate, T entity)
    {
        context.Set<T>().Attach(entityToUpdate);
        context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }
}