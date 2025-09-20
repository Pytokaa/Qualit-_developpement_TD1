namespace TD1.DTO;

public class ProduitDetailDTO
{
    public int IdProduit { get; set; }
    public string? NomProduit { get; set; }
    public string? NomTypeProduit { get; set; }
    public string? NomMarque { get; set; }
    public string? Description { get; set; }
    public string? NomPhoto { get; set; }
    public string? UriPhoto { get; set; }
    public int? StockReel { get; set; }
    public bool? InSupply { get; set; }
    
}