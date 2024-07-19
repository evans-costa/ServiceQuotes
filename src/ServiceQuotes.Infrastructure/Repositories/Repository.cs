using Microsoft.EntityFrameworkCore;
using ServiceQuotes.Domain.Interfaces;
using ServiceQuotes.Infrastructure.Context;
using System.Linq.Expressions;

namespace ServiceQuotes.Infrastructure.Repositories;

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
        return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public async Task<T?> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);

        return entity;
    }

    public T Update(T entity)
    {
        _context.Set<T>().Update(entity);

        return entity;
    }

}
