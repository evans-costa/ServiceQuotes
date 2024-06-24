using ServiceQuotes.Context;
using ServiceQuotes.Repositories.Interfaces;

namespace ServiceQuotes.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ICustomerRepository? _customerRepository;
    private IProductRepository? _productRepository;
    public ServiceQuoteApiContext _context;

    public UnitOfWork(ServiceQuoteApiContext context)
    {
        _context = context;
    }

    public ICustomerRepository CustomerRepository
    {
        get
        {
            return _customerRepository ??= new CustomerRepository(_context);
        }
    }

    public IProductRepository ProductRepository
    {
        get
        {
            return _productRepository ??= new ProductRepository(_context);
        }
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
