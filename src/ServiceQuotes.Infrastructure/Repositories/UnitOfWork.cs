using ServiceQuotes.Domain.Interfaces;
using ServiceQuotes.Infrastructure.Context;

namespace ServiceQuotes.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ICustomerRepository? _customerRepository;
    private IProductRepository? _productRepository;
    private IQuotesRepository? _quotesRepository;
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

    public IQuotesRepository QuotesRepository
    {
        get
        {
            return _quotesRepository ??= new QuotesRepository(_context);
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
