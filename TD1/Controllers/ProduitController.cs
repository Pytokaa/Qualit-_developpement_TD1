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

    // GET: api/Produits
    /// <summary>
    /// Get the list of products
    /// </summary>
    /// <returns>An HTTP response containing the list of products.</returns>
    /// <response code="200">The request was successful and returned a list of products</response>
    /// <response code="500">An internal server error occurred.</response>
    /// 
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Produit>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Produit>>> GetAll()
    {
        return await _productManager.GetAllAsync();
    }
    // GET: api/Produits/id/5
    /// <summary>
    /// Retrieves a specific product by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product to retrieve.</param>
    /// <returns>An HTTP response containing the product corresponding to the identifier.</returns>
    /// <response code="200">The product was found and is returned.</response>
    /// <response code="404">No product was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Produit),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Produit>> GetById(int id)
    {
        var produit = await _productManager.GetByIdAsync(id);
        
        if (produit.Value == null)
        {
            return NotFound();
        }
        return produit;
    }
    // GET: api/Produits/id/{id}
    /// <summary>
    /// Retrieves a specific product by its name.
    /// </summary>
    /// <param name="name">The name of the product to retrieve.</param>
    /// <returns>An HTTP response containing the product corresponding to the name.</returns>
    /// <response code="200">The product was found and is returned.</response>
    /// <response code="404">No product was found with the specified name.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(Produit),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Produit>> GetByName(string name)
    {
        var produit = await _productManager.GetByStringAsync(name);
        if (produit.Value == null)
        {
            return NotFound();
        }

        return produit;
    }

    
    // POST: api/Produits
    /// <summary>
    /// Adds a new product to the database.
    /// </summary>
    /// <param name="produit">The product data to add.</param>
    /// <returns>An HTTP response containing the created product.</returns>
    /// <response code="201">The product was successfully created.</response>
    /// <response code="400">The provided data is invalid or incomplete.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Produit), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Produit>> AddProduit(Produit produit)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await _productManager.AddAsync(produit);
        return CreatedAtAction( nameof(GetById), new { id = produit.IdProduit }, produit);
    }

    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    // DELETE: api/Produits/{id}
    /// <summary>
    /// Deletes an existing product by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product to delete.</param>
    /// <returns>An HTTP response indicating the result of the operation.</returns>
    /// <response code="204">The product was successfully deleted.</response>
    /// <response code="404">No product was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>

    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    
    public async Task<IActionResult> DeleteProduit(int id) //y'a un truc qui va pas ici, a revoir
    {
        ActionResult<Produit?> produitToDelete = await _productManager.GetByIdAsync(id);
        if (produitToDelete.Value == null)
        {
            return NotFound();
        }
        await _productManager.DeleteAsync(produitToDelete.Value);
        return NoContent();
    }
}