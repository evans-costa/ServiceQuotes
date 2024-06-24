namespace ServiceQuotes.Repositories.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository CustomerRepository { get; }
    IProductRepository ProductRepository { get; }
    Task CommitAsync();
    Task DisposeAsync();
}
