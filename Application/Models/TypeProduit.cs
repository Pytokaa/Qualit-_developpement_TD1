using Application.Attributes;

namespace Application.Models;

public class TypeProduit :  IEntity
{
    [IgnoreInTemplate]
    public int IdTypeProduit { get; set; }
  
    public string NomTypeProduit { get; set; }
    

    public int? GetId() =>  IdTypeProduit;
    public string? GetName() =>  NomTypeProduit;
}