using AutoMapper;
using ServiceQuotes.API.DTOs.Customer;
using ServiceQuotes.API.Models;

namespace ServiceQuotes.API.Mappings;

public class CustomerDTOMappingProfile : Profile
{
    public CustomerDTOMappingProfile()
    {
        CreateMap<Customer, CustomerRequestDTO>().ReverseMap();
        CreateMap<Customer, CustomerResponseDTO>().ReverseMap();
    }
}
