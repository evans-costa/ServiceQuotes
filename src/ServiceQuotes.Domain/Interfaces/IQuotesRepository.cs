using ServiceQuotes.Domain.Entities;
using ServiceQuotes.Domain.Pagination;

namespace ServiceQuotes.Domain.Interfaces;

public interface IQuotesRepository : IRepository<Quote>
{
    Task<IEnumerable<Quote>> GetQuotesAsync();
    Task<Quote?> GetDetailedQuoteAsync(int id);
    Task<IEnumerable<Quote>> SearchQuotesAsync(QuoteFilterParams quoteParams);
}
