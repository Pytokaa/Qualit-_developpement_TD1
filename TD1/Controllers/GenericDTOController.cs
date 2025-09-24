using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TD1.DTO;
using TD1.Models.EntityFramework;
using TD1.Repository;

namespace TD1.Controllers;


[ApiController]
[Route("api/[controller]/[action]")]
public abstract class GenericDTOController<T, TDto, TDetailDto> : ControllerBase where T : class, IEntity where TDto : class
    where TDetailDto : class,  IDtoEntity
{
    protected readonly IDataRepository<T> _manager;
    protected readonly IMapper _mapper;

    public GenericDTOController(IDataRepository<T> manager, IMapper mapper)
    {
        _manager = manager;
        _mapper = mapper;
    }
    
    
    [HttpGet]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TDto>>> GetAll()
    {
        var entities = (await _manager.GetAllAsync()).Value;
        var entitiesDTO = _mapper.Map<IEnumerable<TDto>>(entities);

        return new ActionResult<IEnumerable<TDto>>(entitiesDTO);
    }
    
    [HttpGet("id/{id}")]
    public new async Task<ActionResult<TDetailDto>> GetById(int id)
    {
        var entity = await _manager.GetByIdAsync(id);
        if (entity.Value == null)
            return NotFound();
        
        var entityDTO = _mapper.Map<TDetailDto>(entity.Value);
        return entityDTO;
    }
    [HttpGet("name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TDetailDto>> GetByName(string name)
    {
        var entity = await _manager.GetByStringAsync(name);
        if (entity.Value == null)
        {
            return NotFound();
        }
        var entityDTO = _mapper.Map<TDetailDto>(entity.Value);
        return entityDTO;
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TDetailDto>> Add([FromBody]TDetailDto entityDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (entityDto.GetId() != null)
        {
            entityDto.SetId();
        }
        
        var entity = _mapper.Map<T>(entityDto);
        await _manager.AddAsync(entity);
        
        
        var resultDto = _mapper.Map<TDetailDto>(entity);

        return CreatedAtAction(
            nameof(GetById),
            new { id = entity.GetId() },
            resultDto
        );
    }
    
        
    
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put(int id, [FromBody] TDetailDto  entityDto)
    {
        if (entityDto.GetId() != null)
        {
            if (id != entityDto.GetId())
            {
                return BadRequest();
            }
        }
        
        ActionResult<T?> entityToUpdate = await _manager.GetByIdAsync(id);

        if (entityToUpdate.Value == null)
        {
            return NotFound();
        }
        var entity = _mapper.Map<T>(entityDto);
        await _manager.UpdateAsync(entityToUpdate.Value, entity);
        return NoContent();
    }
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id) //y'a un truc qui va pas ici, a revoir
    {
        ActionResult<T?> produitToDelete = await _manager.GetByIdAsync(id);
        if (produitToDelete.Value == null)
        {
            return NotFound();
        }
        await _manager.DeleteAsync(produitToDelete.Value);
        return NoContent();
    }
}