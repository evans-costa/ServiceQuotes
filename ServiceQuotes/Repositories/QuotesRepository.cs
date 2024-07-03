using Microsoft.EntityFrameworkCore;
using ServiceQuotes.Context;
using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using ServiceQuotes.Repositories.Interfaces;
using X.PagedList;

namespace ServiceQuotes.Repositories;

public class QuotesRepository : Repository<Quote>, IQuotesRepository
{
    public QuotesRepository(ServiceQuoteApiContext context) : base(context)
    {
    }

    public async Task<Quote?> GetDetailedQuoteAsync(int id)
    {
        var detailedQuote = await _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.Products)
            .ThenInclude(p => p.QuoteProducts
                .Where(qp => qp.QuoteId == id)
            )
            .FirstOrDefaultAsync(q => q.QuoteId == id);

        return detailedQuote;
    }

    public async Task<IPagedList<Quote>> SearchQuotesAsync(QuoteFilterParams quoteParams)
    {
        IEnumerable<Quote> quotes = await _context.Quotes
            .Include(q => q.Customer)
            .AsNoTracking()
            .ToListAsync();

        string? filterType = quoteParams.FilterCriteria;

        if (!string.IsNullOrEmpty(filterType))
        {
            quotes = filterType.ToLower() switch
            {
                "date" => GetQuoteByDateAsync(quoteParams, quotes),
                "customer" => GetQuoteByCustomerName(quoteParams, quotes),
                _ => quotes
            };
        }

        var filteredQuotes = await quotes.AsQueryable().ToPagedListAsync(quoteParams.PageNumber, quoteParams.PageSize);

        return filteredQuotes;
    }

    private static IEnumerable<Quote> GetQuoteByDateAsync(QuoteFilterParams quoteParams, IEnumerable<Quote> quotes)
    {
        if (DateTime.TryParse(quoteParams.CreatedDate, out DateTime createdDate))
        {
            quotes = quotes.Where(q => q.CreatedAt.Date == createdDate.Date).OrderBy(q => q.CreatedAt);
        }

        return quotes;
    }

    private static IEnumerable<Quote> GetQuoteByCustomerName(QuoteFilterParams quoteParams, IEnumerable<Quote> quotes)
    {
        if (!string.IsNullOrEmpty(quoteParams.CustomerName))
        {
            quotes = quotes.Where(q => q.Customer!.Name!.Contains(quoteParams.CustomerName, StringComparison.CurrentCultureIgnoreCase)).OrderBy(q => q.CreatedAt);
        };

        return quotes;
    }
}
