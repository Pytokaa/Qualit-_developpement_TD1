using Microsoft.AspNetCore.Mvc;
using TD1.Models;
using TD1.Repository;

namespace TD1.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class TypeProduitController : ControllerBase
{
    private readonly IDataRepository<TypeProduit> _productTypeManager;

    public TypeProduitController(IDataRepository<TypeProduit> manager)
    {
        _productTypeManager = manager;
    }
    // GET: api/TypeProduit
    /// <summary>
    /// Get the list of product types
    /// </summary>
    /// <returns>An HTTP response containing the list of product types.</returns>
    /// <response code="200">The request was successful and returned a list of product types</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TypeProduit>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TypeProduit>>> GetAll()
    {
        return await _productTypeManager.GetAllAsync();
    }
    // GET: api/TYpeProduit/id/5
    /// <summary>
    /// Retrieves a specific product type by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product to retrieve.</param>
    /// <returns>An HTTP response containing the product corresponding to the identifier.</returns>
    /// <response code="200">The product type was found and is returned.</response>
    /// <response code="404">No product type was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(TypeProduit),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TypeProduit>> GetById(int id)
    {
        ActionResult<TypeProduit?> typeProduct = await _productTypeManager.GetByIdAsync(id);
        
        if (typeProduct.Value == null)
        {
            return NotFound();
        }
        return typeProduct;
    }
    // GET: api/TypeProduit/id/{id}
    /// <summary>
    /// Retrieves a specific product type by its name.
    /// </summary>
    /// <param name="name">The name of the product type to retrieve.</param>
    /// <returns>An HTTP response containing the product corresponding to the name.</returns>
    /// <response code="200">The product type was found and is returned.</response>
    /// <response code="404">No product type was found with the specified name.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(TypeProduit),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TypeProduit>> GetByName(string name)
    {
        var typeProduct = await _productTypeManager.GetByStringAsync(name);
        if (typeProduct.Value == null)
        {
            return NotFound();
        }

        return typeProduct;
    }
    // POST: api/TypeProduit
    /// <summary>
    /// Adds a new product type to the database.
    /// </summary>
    /// <param name="productType">The product data to add.</param>
    /// <returns>An HTTP response containing the created product.</returns>
    /// <response code="201">The product was successfully created.</response>
    /// <response code="400">The provided data is invalid or incomplete.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TypeProduit), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TypeProduit>> AddMarque(TypeProduit productType)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await _productTypeManager.AddAsync(productType);
        return CreatedAtAction( nameof(GetById), new { id = productType.IdTypeProduit }, productType);
    }
    
    // PUT: api/TypeProduits/id/{id}
    /// <summary>
    /// Updates an existing product type by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product type to update.</param>
    /// <param name="productType">The new data for the product type.</param>
    /// <returns>An HTTP response indicating the result of the operation.</returns>
    /// <response code="204">The product type was successfully updated.</response>
    /// <response code="400">The provided data is invalid or the identifier does not match the product type.</response>
    /// <response code="404">No product type was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>

    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutMarque(int id, [FromBody] TypeProduit productType)
    {
        if (id != productType.IdTypeProduit)
        {
            return BadRequest();
        }
        ActionResult<TypeProduit?> prodToUpdate = await _productTypeManager.GetByIdAsync(id);

        if (prodToUpdate.Value == null)
        {
            return NotFound();
        }
        await _productTypeManager.UpdateAsync(prodToUpdate.Value, productType);
        return NoContent();
    }
    // DELETE: api/TypeProduit/{id}
    /// <summary>
    /// Deletes an existing product type by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product type to delete.</param>
    /// <returns>An HTTP response indicating the result of the operation.</returns>
    /// <response code="204">The product type was successfully deleted.</response>
    /// <response code="404">No product type was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMarque(int id) //y'a un truc qui va pas ici, a revoir
    {
        ActionResult<TypeProduit?> productTypeToDelete = await _productTypeManager.GetByIdAsync(id);
        if (productTypeToDelete.Value == null)
        {
            return NotFound();
        }
        await _productTypeManager.DeleteAsync(productTypeToDelete.Value);
        return NoContent();
    }
}