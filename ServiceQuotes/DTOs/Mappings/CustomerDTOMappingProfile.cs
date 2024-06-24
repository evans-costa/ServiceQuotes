using AutoMapper;
using ServiceQuotes.Models;

namespace ServiceQuotes.DTOs.Mappings;

public class CustomerDTOMappingProfile : Profile
{
    public CustomerDTOMappingProfile()
    {
        CreateMap<Customer, CustomerRequestDTO>().ReverseMap();
    }
}
