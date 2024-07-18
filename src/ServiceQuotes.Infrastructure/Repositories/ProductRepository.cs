using ServiceQuotes.Domain.Entities;
using ServiceQuotes.Domain.Interfaces;
using ServiceQuotes.Infrastructure.Context;

namespace ServiceQuotes.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ServiceQuoteApiContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        var products = await GetAllAsync();

        var orderedProducts = products.OrderBy(p => p.Name).AsQueryable();

        return orderedProducts;
    }
}
