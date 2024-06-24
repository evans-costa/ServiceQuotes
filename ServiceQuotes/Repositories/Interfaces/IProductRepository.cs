using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using X.PagedList;

namespace ServiceQuotes.Repositories.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IPagedList<Product>> GetProductsAsync(QueryParameters productParams);
}
