using AutoMapper;
using ServiceQuotes.DTOs.ResponseDTO;
using ServiceQuotes.Models;

namespace ServiceQuotes.DTOs.Mappings;

public class ProductDTOMappingProfile : Profile
{
    public ProductDTOMappingProfile()
    {
        CreateMap<Product, ProductDTO>().ReverseMap();
        CreateMap<Product, ProductDetailDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.QuoteProducts.FirstOrDefault().Price))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QuoteProducts.FirstOrDefault().Quantity))
            .ReverseMap();
    }
}
