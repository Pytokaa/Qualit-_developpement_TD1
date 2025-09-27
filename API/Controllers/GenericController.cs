using Microsoft.AspNetCore.Mvc;
using TD1.Models.EntityFramework;
using TD1.Repository;
/*
namespace API.Controllers;


[ApiController]
[Route("api/[controller]/[action]")]
public abstract class GenericController<T> : ControllerBase where T : class, IEntity
{
    protected readonly IDataRepository<T> _manager;

    public GenericController(IDataRepository<T> manager)
    {
        _manager = manager;
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<T>>> GetAll()
    {
        return await _manager.GetAllAsync();
    }
    
    [HttpGet("id/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<T>> GetById(int id)
    {
        var entity = await _manager.GetByIdAsync(id);
        if (entity.Value == null)
        {
            return NotFound();
        }
        return entity;
    }
    [HttpGet("name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<T>> GetByName(string name)
    {
        var entity = await _manager.GetByStringAsync(name);
        if (entity.Value == null)
        {
            return NotFound();
        }
        return entity;
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<T>> Add(T entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await _manager.AddAsync(entity);
        return CreatedAtAction( nameof(GetById), new { id = entity.GetId() }, entity);
    }
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put(int id, [FromBody] T entity)
    {
        if (id != entity.GetId() && entity.GetId() != null)
        {
            return BadRequest();
        }
        ActionResult<T?> entityToUpdate = await _manager.GetByIdAsync(id);

        if (entityToUpdate.Value == null)
        {
            return NotFound();
        }
        await _manager.UpdateAsync(entityToUpdate.Value, entity);
        return NoContent();
    }
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id) //y'a un truc qui va pas ici, a revoir
    {
        ActionResult<T?> entityToDelete = await _manager.GetByIdAsync(id);
        if (entityToDelete.Value == null)
        {
            return NotFound();
        }
        await _manager.DeleteAsync(entityToDelete.Value);
        return NoContent();
    }
}*/