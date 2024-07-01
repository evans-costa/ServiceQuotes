using AutoMapper;
using ServiceQuotes.DTOs.RequestDTO;
using ServiceQuotes.DTOs.ResponseDTO;
using ServiceQuotes.Models;

namespace ServiceQuotes.DTOs.Mappings;

public class QuoteDTOMappingProfile : Profile
{
    public QuoteDTOMappingProfile()
    {
        CreateMap<Quote, QuoteRequestDTO>().ReverseMap();
        CreateMap<Quote, QuoteResponseDTO>().ReverseMap();

        CreateMap<Quote, QuoteDetailedResponseDTO>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products))
            .ReverseMap();

        CreateMap<QuoteProducts, QuoteProductsRequestDTO>().ReverseMap();
    }
}
