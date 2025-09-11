using Microsoft.AspNetCore.Mvc;
using TD1.Models;
using TD1.Repository;

namespace TD1.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller")]


[ApiController]
[Route("api/[controller]")]
public class ProduitController : ControllerBase
{
    private readonly ProduitManager _productManager;
    
    public ProduitController(ProduitManager manager)
    {
        _productManager = manager;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produit>>> GetAll()
    {
        return await _productManager.GetAllAsync();
    }
    
    [HttpGet("id/{id}")]
    public async Task<ActionResult<Produit>> GetById(int id)
    {
        var produit = await _productManager.GetByIdAsync(id);
        
        if (produit.Value == null)
        {
            return NotFound();
        }
        return produit;
    }

    [HttpPost]
    public async Task<ActionResult<Produit>> AddProduit(Produit produit)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await _productManager.AddAsync(produit);
        return CreatedAtAction("GetById", new { id = produit.IdProduit }, produit);
    }

    [HttpPut("id/{id}")]
    public async Task<IActionResult> PutProduit(int id, Produit produit)
    {
        var produitToUpdate =  await _productManager.GetByIdAsync(id);
        if (produitToUpdate.Value == null)
        {
            return NotFound();
        }
        _productManager.UpdateAsync(produitToUpdate.Value, produit);
        return NoContent();
    }

    [HttpDelete("id/{id}")]
    public async Task<IActionResult> DeleteProduit(int id)
    {
        var produitToDelete = await _productManager.GetByIdAsync(id);
        if (produitToDelete.Value == null)
        {
            return NotFound();
        }
        await _productManager.DeleteAsync(produitToDelete.Value);
        return NoContent();
        
    }
}