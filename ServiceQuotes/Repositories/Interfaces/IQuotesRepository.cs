using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using X.PagedList;

namespace ServiceQuotes.Repositories.Interfaces;

public interface IQuotesRepository : IRepository<Quote>
{
    Task<Quote?> GetDetailedQuoteAsync(int id);
    Task<IPagedList<Quote>> SearchQuotesAsync(QuoteFilterParams quoteParams);
}
