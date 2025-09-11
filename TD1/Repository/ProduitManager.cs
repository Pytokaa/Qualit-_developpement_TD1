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

    public async Task AddAsync(Produit entity)
    {
        await dbContext.Produits.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Produit entityToUpdate, Produit entity)
    {
        dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        
        entityToUpdate.Description = entity.Description;
        entityToUpdate.NomPhoto = entity.NomPhoto;
        entityToUpdate.StockMax = entity.StockMax;
        entityToUpdate.StockMin = entity.StockMin;
        entityToUpdate.StockReel = entity.StockReel;
        entityToUpdate.NomProduit = entity.NomProduit;
        entityToUpdate.UriPhoto = entity.UriPhoto;
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Produit entity)
    {
        dbContext.Produits.Remove(entity);
        await dbContext.SaveChangesAsync();
        
    }
}