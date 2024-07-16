using ServiceQuotes.API.Models;
using ServiceQuotes.API.Pagination;
using X.PagedList;

namespace ServiceQuotes.API.Repositories.Interfaces;

public interface IQuotesRepository : IRepository<Quote>
{
    Task<IPagedList<Quote>> GetQuotesAsync(QueryParameters quoteParams);
    Task<Quote?> GetDetailedQuoteAsync(int id);
    Task<IPagedList<Quote>> SearchQuotesAsync(QuoteFilterParams quoteParams);
}
