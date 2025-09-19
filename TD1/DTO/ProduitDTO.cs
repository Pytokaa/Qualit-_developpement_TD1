namespace TD1.DTO;

public class ProduitDTO
{
    public int Id { get; set; }
    public string? Nom { get; set; }
    public string? Type { get; set; }
    public string? Marque { get; set; }
    public string? Description { get; set; }
    public string? NomPhoto { get; set; }
    public string? UriPhoto { get; set; }
    public int? Stock { get; set; }
    public bool InSupply { get; set; }
    
}