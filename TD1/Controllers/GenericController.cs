using Microsoft.AspNetCore.Mvc;
using TD1.Repository;

namespace TD1.Controllers;


[ApiController]
[Route("api/[controller]/[action]")]
public abstract class GenericController<T>(IDataRepository<T> _manager) : ControllerBase where T : class
{
    [HttpGet("{id")]
    public virtual async Task<ActionResult<T>> GetById(int id)
    {
        var entity = await _manager.GetByIdAsync(id);
        if (entity.Value == null)
        {
            return NotFound();
        }

        return entity;
    }
    
}