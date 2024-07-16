using AutoMapper;
using ServiceQuotes.DTOs.Customer;
using ServiceQuotes.Models;

namespace ServiceQuotes.Mappings;

public class CustomerDTOMappingProfile : Profile
{
    public CustomerDTOMappingProfile()
    {
        CreateMap<Customer, CustomerRequestDTO>().ReverseMap();
        CreateMap<Customer, CustomerResponseDTO>().ReverseMap();
    }
}
