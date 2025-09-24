using System;
using System.Linq;
using System.Reflection;

namespace TD1.DTO;

public class ProduitDetailDTO : IDtoEntity
{
    public int? IdProduit { get; set; }
    public string? NomProduit { get; set; }
    public string? NomTypeProduit { get; set; }
    public string? NomMarque { get; set; }
    public int? IdMarque { get; set; }
    public int? IdTypeProduit { get; set; }
    public string? Description { get; set; }
    public string? NomPhoto { get; set; }
    public string? UriPhoto { get; set; }
    public int? StockReel { get; set; }
    public bool? InSupply { get; set; }
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ProduitDetailDTO)obj;
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var value1 = prop.GetValue(this);
            var value2 = prop.GetValue(other);

            if (value1 == null && value2 == null)
                continue;

            if (value1 == null || value2 == null)
                return false;

            if (!value1.Equals(value2))
                return false;
        }

        return true;
    }

    public int? GetId()=> IdProduit;

    public void SetId()
    {
        IdProduit = null;
    }
}