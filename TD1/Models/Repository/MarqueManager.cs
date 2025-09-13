using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TD1.Models;
using TD1.Models.EntityFramework;


namespace TD1.Repository;

public class MarqueManager(ProduitDbContext context) : IDataRepository<Marque>
{
    public async Task<ActionResult<IEnumerable<Marque>>> GetAllAsync()
    {
        return await context.Marques.ToListAsync();
    }

    public async Task<ActionResult<Marque>> GetByIdAsync(int id)
    {
        var marque = await context.Marques.FirstOrDefaultAsync(e => e.IdMarque == id);
        return marque;
    }

    public async Task<ActionResult<Marque>> GetByStringAsync(string str)
    {
        ActionResult<Marque?> marque = context.Marques.FirstOrDefault(e => e.NomMarque == str);
        return marque;
    }

    public async Task AddAsync(Marque entity)
    {
        await context.Marques.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Marque entityToUpdate, Marque entity)
    {
        context.Marques.Attach(entityToUpdate);
        context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Marque entity)
    {
        context.Marques.Remove(entity);
        await context.SaveChangesAsync();
    }
}