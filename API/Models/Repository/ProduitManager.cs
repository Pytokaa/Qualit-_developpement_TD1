using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TD1.Attributes;
using TD1.Models;
using TD1.Models.EntityFramework;

namespace TD1.Repository;

public class ProductManager : GenericManager<Product>, IFiltrableRepository<Product>
{
    public ProductManager(AppDbContext context) : base(context){}
    
    

    public async Task<ActionResult<IEnumerable<Product>>>FilterAsync(string? name=null, string? brand = null, string? productType = null)
    {
        var query = _context.Produits.IncludeNavigationPropertiesIfNeeded().AsQueryable();
        if (!string.IsNullOrEmpty(name))
            query = query.Where(p => p.NomProduit.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrEmpty(brand))
            query = query.Where(p => p.MarqueNavigation.NomMarque == brand);

        if (!string.IsNullOrEmpty(productType))
            query = query.Where(p => p.TypeProduitNavigation.NomTypeProduit == productType);
        
        var result = await query.ToListAsync();

        return new ActionResult<IEnumerable<Product>>(result);
    }
}