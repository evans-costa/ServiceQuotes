using Microsoft.EntityFrameworkCore;
using ServiceQuotes.Context;
using ServiceQuotes.Models;
using ServiceQuotes.Repositories.Interfaces;

namespace ServiceQuotes.Repositories;

public class QuotesRepository : Repository<Quote>, IQuotesRepository
{
    public QuotesRepository(ServiceQuoteApiContext context) : base(context)
    {
    }

    public async Task<Quote?> GetDetailedQuoteAsync
        (int id)
    {
        var detailedQuote = await _context.Quotes
            .Include(q => q.Products)
            .ThenInclude(p => p.QuoteProducts
                .Where(qp => qp.QuoteId == id)
            )
            .FirstOrDefaultAsync(q => q.QuoteId == id);

        return detailedQuote;
    }

    public async Task<bool>
        IsProductAssociatedWithQuote(Guid productId, int quoteId)
    {
        var result = await _context.Quotes
            .Include(q => q.QuotesProducts)
            .Where(q => q.QuoteId == quoteId)
            .SelectMany(q => q.QuotesProducts)
            .AnyAsync(p => p.ProductId == productId);

        return result;
    }
}
