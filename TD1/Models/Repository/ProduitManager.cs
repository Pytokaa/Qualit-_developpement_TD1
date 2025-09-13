using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TD1.Models;
using TD1.Models.EntityFramework;

namespace TD1.Repository;

public class ProduitManager(ProduitDbContext context) : IDataRepository<Produit>
{
    
    public async Task<ActionResult<IEnumerable<Produit>>> GetAllAsync()
    {
        return await context.Produits.ToListAsync();
    }

    public async Task<ActionResult<Produit>> GetByIdAsync(int id)
    {
        var produit = await context.Produits.FirstOrDefaultAsync(e => e.IdProduit == id);
        return produit;
    }

    public async Task<ActionResult<Produit>> GetByStringAsync(string str)
    {
        return await context.Produits.FirstOrDefaultAsync(u => u.NomProduit.ToUpper() == str.ToUpper());
    }

    public async Task AddAsync(Produit entity)
    {
        await context.Produits.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Produit entityToUpdate, Produit entity)
    {
        context.Produits.Attach(entityToUpdate);
        context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Produit entity)
    {
        context.Produits.Remove(entity);
        await context.SaveChangesAsync();
    }
}