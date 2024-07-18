using AutoMapper;
using ServiceQuotes.Application.DTO.Quote;
using ServiceQuotes.Domain.Entities;

namespace ServiceQuotes.Application.Mappings;

public class QuoteDTOMappingProfile : Profile
{
    public QuoteDTOMappingProfile()
    {
        CreateMap<Quote, QuoteRequestDTO>().ReverseMap();
        CreateMap<Quote, QuoteResponseDTO>().ReverseMap();

        CreateMap<Quote, QuoteDetailedResponseDTO>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products))
            .ForMember(dest => dest.CustomerInfo, opt => opt.MapFrom(src => src.Customer))
            .ReverseMap();

        CreateMap<QuoteProducts, QuoteProductsRequestDTO>().ReverseMap();
    }
}
