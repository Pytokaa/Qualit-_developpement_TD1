using Application.Attributes;

namespace Application.Models;

public class Product : IEntity
{
    [IgnoreInTemplate]
    public int IdProduit { get; set; }
    public string? NomProduit { get; set; } = null!;
    [IgnoreInTemplate]
    public string? NomMarque { get; set; } = null!;
    [IgnoreInTemplate]
    public string? NomTypeProduit { get; set; } = null!;
    [IgnoreInTemplate]
    public int? IdMarque { get; set; } = null!;
    [IgnoreInTemplate]
    public int? IdTypeProduit { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public string? NomPhoto { get; set; } = null!;
    public string? UriPhoto { get; set; } = null!;
    public int? StockReel { get; set; }
    public int? StockMin { get; set; }
    public int? StockMax { get; set; }

    public int? GetId() =>   IdProduit;
    public string? GetName() =>  NomProduit;
}