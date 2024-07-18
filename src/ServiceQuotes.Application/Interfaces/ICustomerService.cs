using ServiceQuotes.Application.DTO.Customer;
using ServiceQuotes.Domain.Pagination;

namespace ServiceQuotes.Application.Interfaces;
public interface ICustomerService
{
    Task<(IEnumerable<CustomerResponseDTO>, object)> GetAllCustomers(QueryParameters customerParams);
    Task<CustomerResponseDTO> GetCustomerById(Guid id);
    Task<CustomerResponseDTO> CreateCustomer(CustomerRequestDTO customerDto);

}
