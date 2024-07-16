using ServiceQuotes.Context;
using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using ServiceQuotes.Repositories.Interfaces;
using X.PagedList;

namespace ServiceQuotes.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ServiceQuoteApiContext context) : base(context)
    {
    }

    public async Task<IPagedList<Product>> GetProductsAsync(QueryParameters productParams)
    {
        var products = await GetAllAsync();

        var orderedProducts = products.OrderBy(p => p.Name).AsQueryable();

        var result = await orderedProducts.ToPagedListAsync(productParams.PageNumber, productParams.PageSize);

        return result;
    }
}
