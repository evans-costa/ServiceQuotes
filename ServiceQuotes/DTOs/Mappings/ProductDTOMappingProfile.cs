using AutoMapper;
using ServiceQuotes.Models;

namespace ServiceQuotes.DTOs.Mappings;

public class ProductDTOMappingProfile : Profile
{
    public ProductDTOMappingProfile()
    {
        CreateMap<Product, ProductRequestDTO>().ReverseMap();
    }
}
