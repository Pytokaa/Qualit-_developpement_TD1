
namespace TD1.Mapper;

public interface IMapper<Entity, DTO> //AUTOMAPPER !!!!!
{
    public Entity? FromDTO(DTO dto);
    public DTO? FromEntity(Entity entity);

    IEnumerable<DTO> ToDTO(IEnumerable<Entity> entities)
    { 
        return entities.Select(e => FromEntity(e));
    }
    IEnumerable<Entity> ToEntity(IEnumerable<DTO> dtos)
    { 
        return dtos.Select(e => FromDTO(e));
    }
}