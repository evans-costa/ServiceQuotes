using AutoMapper;
using ServiceQuotes.API.DTOs.Product;
using ServiceQuotes.API.Models;

namespace ServiceQuotes.API.Mappings;

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
