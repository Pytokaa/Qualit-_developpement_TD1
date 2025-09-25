namespace Application.Models;

public interface IEntity
{
    public int? GetId();
    public string? GetName();
}