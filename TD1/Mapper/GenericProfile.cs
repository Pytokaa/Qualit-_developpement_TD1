using AutoMapper;
using TD1.DTO;
using TD1.Models;

namespace TD1.Mapper;

public class GenericProfile : Profile
{
    public GenericProfile()
    {
        //creation des mappings pour produit
        CreateMap<Produit, ProduitDTO>()
            .ForMember(dest => dest.NomMarque,
                opt => opt.MapFrom(src => src.MarqueNavigation != null 
                    ? src.MarqueNavigation.NomMarque 
                    : string.Empty))
            .ForMember(dest => dest.NomTypeProduit,
                opt => opt.MapFrom(src => src.TypeProduitNavigation != null 
                    ? src.TypeProduitNavigation.NomTypeProduit 
                    : string.Empty));
        CreateMap<ProduitDTO, Produit>();
        
        CreateMap<Produit, ProduitDetailDTO>()
            .ForMember(dest => dest.NomMarque,
                opt => opt.MapFrom(src => src.MarqueNavigation != null 
                    ? src.MarqueNavigation.NomMarque 
                    : string.Empty))
            .ForMember(dest => dest.NomTypeProduit,
                opt => opt.MapFrom(src => src.TypeProduitNavigation != null 
                    ? src.TypeProduitNavigation.NomTypeProduit 
                    : string.Empty));
        CreateMap<ProduitDetailDTO, Produit>();
        
        
    }
}