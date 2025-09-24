namespace Application.Models;

public class Marque : IEntity
{
    public int? IdMarque { get; set; }
    public string? NomMarque { get; set; }
    
    public int? GetId() =>  IdMarque;
}