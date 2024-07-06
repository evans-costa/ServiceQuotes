using AutoMapper;
using ServiceQuotes.DTOs.Product;
using ServiceQuotes.Models;

namespace ServiceQuotes.Mappings;

public class ProductDTOMappingProfile : Profile
{
    public ProductDTOMappingProfile()
    {
        CreateMap<Product, ProductResponseDTO>().ReverseMap();
        CreateMap<Product, ProductRequestDTO>().ReverseMap();

        CreateMap<Product, ProductDetailResponseDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.QuoteProducts.FirstOrDefault()!.Price))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QuoteProducts.FirstOrDefault()!.Quantity))
            .ReverseMap();
    }
}
