using AutoMapper;
using TD1.DTO;
using TD1.Models;

namespace TD1.Mapper;

public class GenericProfile : Profile
{
    public GenericProfile()
    {
        //creation des mappings pour produit
        CreateMap<Produit, ProduitDTO>();
        CreateMap<ProduitDTO, Produit>();
        
        CreateMap<Produit, ProduitDetailDTO>();
        CreateMap<ProduitDetailDTO, Produit>();
        
        //creation des mappings pour Marque
        CreateMap<Marque, MarqueDTO>();
        CreateMap<MarqueDTO, Marque>();
        
        //mappings pour TypeProduit
        CreateMap<TypeProduit, TypeProduitDTO>();
        CreateMap<TypeProduitDTO, TypeProduit>();
    }
}