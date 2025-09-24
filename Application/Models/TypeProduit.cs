namespace Application.Models;

public class TypeProduit :  IEntity
{
    public int?  IdTypeProduit { get; set; }
    public string? NomTypeProduit { get; set; }

    public int? GetId() =>  IdTypeProduit;
}