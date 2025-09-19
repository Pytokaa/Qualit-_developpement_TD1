using TD1.DTO;
using TD1.Models;

namespace TD1.Mapper;

public class ProduitMapper : IMapper<Produit,  ProduitDTO>
{
    public Produit? FromDTO(ProduitDTO dto)
    {
        return new Produit()
        {
            IdProduit = dto.Id,
            NomProduit = dto.Nom,
            TypeProduitNavigation = new TypeProduit(){NomTypeProduit = dto.Type},
            MarqueNavigation = new Marque(){NomMarque = dto.Marque},
            Description = dto.Description,
            NomPhoto = dto.NomPhoto,
            UriPhoto = dto.UriPhoto,
            StockReel = dto.Stock
        };
    }

    public ProduitDTO? FromEntity(Produit entity)
    {
        return new ProduitDTO()
        {
            Id = entity.IdProduit,
            Nom = entity.NomProduit,
            Marque = entity.MarqueNavigation.NomMarque,
            Type = entity.TypeProduitNavigation.NomTypeProduit,
            Description = entity.Description,
            NomPhoto = entity.NomPhoto,
            UriPhoto = entity.UriPhoto,
            Stock = entity.StockReel,
            InSupply = entity.StockReel > 0.7,
        };
    }
}

