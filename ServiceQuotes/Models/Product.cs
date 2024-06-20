using System.Collections.ObjectModel;

namespace ServiceQuotes.Models;

public class Product
{
    public Product()
    {
        Quotes = new Collection<Quote>();
        QuotesProducts = new Collection<QuoteProducts>();
    }
    public Guid ProductId { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Quote> Quotes { get; }
    public ICollection<QuoteProducts>? QuotesProducts { get; }
}
