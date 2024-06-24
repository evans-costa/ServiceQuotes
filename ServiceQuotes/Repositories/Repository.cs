using Microsoft.EntityFrameworkCore;
using ServiceQuotes.Context;
using ServiceQuotes.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ServiceQuotes.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ServiceQuoteApiContext _context;
    public Repository(ServiceQuoteApiContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public T? Create(T entity)
    {
        _context.Set<T>().Add(entity);

        return entity;
    }

}
