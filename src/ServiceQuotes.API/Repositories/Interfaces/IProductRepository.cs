using ServiceQuotes.API.Models;
using ServiceQuotes.API.Pagination;
using X.PagedList;

namespace ServiceQuotes.API.Repositories.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IPagedList<Product>> GetProductsAsync(QueryParameters productParams);
}
