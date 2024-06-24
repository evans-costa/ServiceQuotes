using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using X.PagedList;

namespace ServiceQuotes.Repositories.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IPagedList<Customer>> GetCustomersAsync(QueryParameters customerParams);
}
