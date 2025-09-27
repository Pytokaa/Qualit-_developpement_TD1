using Application.Attributes;

namespace Application.Models;

public class Marque : IEntity
{
    [IgnoreInTemplate]
    public int IdMarque { get; set; }
    public string? NomMarque { get; set; }
    
    public int? GetId() =>  IdMarque;
    public string? GetName() =>  NomMarque;
}