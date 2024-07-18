namespace ServiceQuotes.Domain.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository CustomerRepository { get; }
    IProductRepository ProductRepository { get; }
    IQuotesRepository QuotesRepository { get; }
    Task CommitAsync();
    Task DisposeAsync();
}
