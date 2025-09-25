using Application.Attributes;

namespace Application.Models;

public class Product : IEntity
{
    public int IdProduit { get; set; }
    public string? NomProduit { get; set; } = null!;
    public string? NomMarques { get; set; } = null!;
    public string? NomTypeProduit { get; set; } = null!;
    
    public int? IdMarque { get; set; } = null!;
    public int? IdTypeProduit { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public string? NomPhoto { get; set; } = null!;
    public string? UriPhoto { get; set; } = null!;

    public int? GetId() =>   IdProduit;
    public string? GetName() =>  NomProduit;
}