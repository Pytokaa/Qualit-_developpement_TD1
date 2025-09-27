using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TD1.Models;
using TD1.Models.EntityFramework;

namespace TD1.Repository;
/*
public class TypeProduitManager(ProduitDbContext context) : IDataRepository<TypeProduit>
{
    public async Task<ActionResult<IEnumerable<TypeProduit>>> GetAllAsync()
    {
        return await context.TypeProduits.ToListAsync();
    }

    public async Task<ActionResult<TypeProduit>> GetByIdAsync(int id)
    {
        var typeProduit = await context.TypeProduits.FirstOrDefaultAsync(e => e.IdTypeProduit == id);
        return typeProduit;
    }

    public async Task<ActionResult<TypeProduit>> GetByStringAsync(string str)
    {
        return await context.TypeProduits.FirstOrDefaultAsync(e => e.NomTypeProduit == str);
    }

    public async Task AddAsync(TypeProduit entity)
    {
        await context.TypeProduits.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TypeProduit entityToUpdate, TypeProduit entity)
    {
        context.TypeProduits.Attach(entityToUpdate);
        context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TypeProduit entity)
    {
        context.TypeProduits.Remove(entity);
        await context.SaveChangesAsync();
    }
}*/

public class TypeProduitManager : GenericManager<TypeProduit>
{
    public TypeProduitManager(AppDbContext context) : base(context){}
}