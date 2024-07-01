using ServiceQuotes.Models;

namespace ServiceQuotes.Repositories.Interfaces;

public interface IQuotesRepository : IRepository<Quote>
{
    Task<Quote?> GetDetailedQuoteAsync(int id);
    Task<bool> IsProductAssociatedWithQuote(Guid productId, int quoteId);
}
