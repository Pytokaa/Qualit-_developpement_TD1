using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TD1.Models;
using TD1.Models.EntityFramework;

namespace TD1.Repository;
/*
public class ProduitManager(ProduitDbContext context) : IDataRepository<Product>
{
    
    public async Task<ActionResult<IEnumerable<Product>>> GetAllAsync()
    {
        return await context.Produits.ToListAsync();
    }

    public async Task<ActionResult<Product>> GetByIdAsync(int id)
    {
        var produit = await context.Produits.FirstOrDefaultAsync(e => e.IdProduit == id);
        return produit;
    }

    public async Task<ActionResult<Product>> GetByStringAsync(string str)
    {
        return await context.Produits.FirstOrDefaultAsync(u => u.NomProduit.ToUpper() == str.ToUpper());
    }

    public async Task AddAsync(Product entity)
    {
        await context.Produits.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product entityToUpdate, Product entity)
    {
        context.Produits.Attach(entityToUpdate);
        context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product entity)
    {
        context.Produits.Remove(entity);
        await context.SaveChangesAsync();
    }
}*/


public class ProductManager : GenericManager<Product>
{
    public ProductManager(AppDbContext context) : base(context){}
}