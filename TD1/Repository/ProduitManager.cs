using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TD1.Models;
using TD1.Models.EntityFramework;

namespace TD1.Repository;

public class ProduitManager : IDataRepository<Produit>
{
    private readonly ProduitDbContext dbContext;
    
    public  ProduitManager(){}

    public ProduitManager(ProduitDbContext? context)
    {
        dbContext = context;
    }
    
    public async Task<ActionResult<IEnumerable<Produit>>> GetAllAsync()
    {
        return await dbContext.Produits.ToListAsync();
    }

    public async Task<ActionResult<Produit>> GetByIdAsync(int id)
    {
        var produit = await dbContext.Produits.FirstOrDefaultAsync(e => e.IdProduit == id);
        return produit;
    }

    public async Task<ActionResult<Produit>> GetByStringAsync(string str)
    {
        return await dbContext.Produits.FirstOrDefaultAsync(u => u.NomProduit.ToUpper() == str.ToUpper());
    }

    public async Task AddAsync(Produit entity)
    {
        await dbContext.Produits.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Produit entityToUpdate, Produit entity)
    {
        dbContext.Produits.Attach(entityToUpdate);
        dbContext.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Produit entity)
    {
        dbContext.Produits.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}