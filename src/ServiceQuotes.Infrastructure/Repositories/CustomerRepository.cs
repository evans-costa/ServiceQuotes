using ServiceQuotes.Domain.Entities;
using ServiceQuotes.Domain.Interfaces;
using ServiceQuotes.Infrastructure.Context;

namespace ServiceQuotes.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ServiceQuoteApiContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        var customers = await GetAllAsync();

        var orderedCustomers = customers.OrderBy(c => c.Name).AsQueryable();

        return orderedCustomers;
    }
}
