using ServiceQuotes.API.Models;
using ServiceQuotes.API.Pagination;
using X.PagedList;

namespace ServiceQuotes.API.Repositories.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IPagedList<Customer>> GetCustomersAsync(QueryParameters customerParams);
}
