using AutoMapper;
using ServiceQuotes.Application.DTO.Product;
using ServiceQuotes.Domain.Entities;

namespace ServiceQuotes.Application.Mappings;

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
