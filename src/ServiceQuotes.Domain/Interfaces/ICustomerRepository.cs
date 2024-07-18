using ServiceQuotes.Domain.Entities;

namespace ServiceQuotes.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> GetCustomersAsync();
}
