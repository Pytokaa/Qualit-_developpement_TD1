using Microsoft.AspNetCore.Mvc;
using TD1.Models;
using TD1.Repository;
using AutoMapper;
using TD1.DTO;

namespace TD1.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller")]
[ApiController]
[Route("api/[controller]")]
public class MarqueController :  ControllerBase
{
    private readonly IDataRepository<Marque> _brandManager;

    public MarqueController(IDataRepository<Marque> manager)
    {
        _brandManager = manager;
    }
    // GET: api/Marques
    /// <summary>
    /// Get the list of brands.
    /// </summary>
    /// <returns>An HTTP response containing the list of brands.</returns>
    /// <response code="200">The request was successful and returned a list of brands.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Marque>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Marque>>> GetAll()
    {
        return  await _brandManager.GetAllAsync();
    }
    // GET: api/Marques/id/5
    /// <summary>
    /// Retrieves a specific brand by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the brand to retrieve.</param>
    /// <returns>An HTTP response containing the brand corresponding to the identifier.</returns>
    /// <response code="200">The brand was found and is returned.</response>
    /// <response code="404">No brand was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Marque),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Marque>> GetById(int id)
    {
        var brand = await _brandManager.GetByIdAsync(id);
        if (brand.Value == null)
        {
            return NotFound();
        }
        return brand;
    }
    // GET: api/Marques/name/{name}
    /// <summary>
    /// Retrieves a specific brand by its name.
    /// </summary>
    /// <param name="name">The name of the brand to retrieve.</param>
    /// <returns>An HTTP response containing the brand corresponding to the name.</returns>
    /// <response code="200">The brand was found and is returned.</response>
    /// <response code="404">No brand was found with the specified name.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(Marque),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Marque>> GetByName(string name)
    {
        var brand = await _brandManager.GetByStringAsync(name);
        if (brand.Value == null)
        {
            return NotFound();
        }
        return brand;
    }
    // POST: api/Marques
    /// <summary>
    /// Adds a new brand to the database.
    /// </summary>
    /// <param name="brand">The brand data to add.</param>
    /// <returns>An HTTP response containing the created brand.</returns>
    /// <response code="201">The brand was successfully created.</response>
    /// <response code="400">The provided data is invalid or incomplete.</response>
    /// <response code="500">An internal server error occurred.</response>

    [HttpPost]
    [ProducesResponseType(typeof(Marque), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Marque>> AddMarque(Marque brand)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await _brandManager.AddAsync(brand);
        return CreatedAtAction( nameof(GetById), new { id = brand.IdMarque }, brand);
    }
    // PUT: api/Marques/id/{id}
    /// <summary>
    /// Updates an existing brand by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the brand to update.</param>
    /// <param name="brand">The new data for the brand.</param>
    /// <returns>An HTTP response indicating the result of the operation.</returns>
    /// <response code="204">The brand was successfully updated.</response>
    /// <response code="400">The provided data is invalid or the identifier does not match the brand.</response>
    /// <response code="404">No brand was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutMarque(int id, [FromBody] Marque brand)
    {
        if (id != brand.IdMarque)
        {
            return BadRequest();
        }
        ActionResult<Marque?> brandToUpdate = await _brandManager.GetByIdAsync(id);

        if (brandToUpdate.Value == null)
        {
            return NotFound();
        }
        await _brandManager.UpdateAsync(brandToUpdate.Value, brand);
        return NoContent();
    }
    // DELETE: api/Marques/{id}
    /// <summary>
    /// Deletes an existing brand by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the brand to delete.</param>
    /// <returns>An HTTP response indicating the result of the operation.</returns>
    /// <response code="204">The brand was successfully deleted.</response>
    /// <response code="404">No brand was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMarque(int id) //y'a un truc qui va pas ici, a revoir
    {
        ActionResult<Marque?> brandToDelete = await _brandManager.GetByIdAsync(id);
        if (brandToDelete.Value == null)
        {
            return NotFound();
        }
        await _brandManager.DeleteAsync(brandToDelete.Value);
        return NoContent();
    }
}