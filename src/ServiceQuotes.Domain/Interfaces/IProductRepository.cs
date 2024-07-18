using ServiceQuotes.Domain.Entities;

namespace ServiceQuotes.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsAsync();
}
