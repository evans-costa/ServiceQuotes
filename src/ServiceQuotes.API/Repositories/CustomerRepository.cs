using ServiceQuotes.API.Context;
using ServiceQuotes.API.Models;
using ServiceQuotes.API.Pagination;
using ServiceQuotes.API.Repositories.Interfaces;
using X.PagedList;

namespace ServiceQuotes.API.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ServiceQuoteApiContext context) : base(context)
    {
    }

    public async Task<IPagedList<Customer>> GetCustomersAsync(QueryParameters customerParams)
    {
        var customers = await GetAllAsync();

        var orderedCustomers = customers.OrderBy(c => c.Name).AsQueryable();

        var result = await orderedCustomers.ToPagedListAsync(customerParams.PageNumber, customerParams.PageSize);

        return result;
    }
}
