using AutoMapper;
using ServiceQuotes.Application.DTO.Customer;
using ServiceQuotes.Domain.Entities;

namespace ServiceQuotes.Application.Mappings;

public class CustomerDTOMappingProfile : Profile
{
    public CustomerDTOMappingProfile()
    {
        CreateMap<Customer, CustomerRequestDTO>().ReverseMap();
        CreateMap<Customer, CustomerResponseDTO>().ReverseMap();
    }
}
