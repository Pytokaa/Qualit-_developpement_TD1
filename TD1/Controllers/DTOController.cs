using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TD1.Repository;

namespace TD1.Controllers;


[ApiController]
[Route("api/[controller]/[action")]
public abstract class DTOController<T, TDto>(IDataRepository<T> _manager, IMapper _mapper) : GenericController<T>(_manager) where T : class
{
    /*
    [HttpGet("{id")]
    public override async Task<IActionResult<TDto>> GetById(int id)
    {
        
    }
    */
}