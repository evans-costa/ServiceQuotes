using System.Collections.ObjectModel;

namespace ServiceQuotes.Models;

public class Quote
{
    public Quote()
    {
        Products = new Collection<Product>();
        QuotesProducts = new Collection<QuoteProducts>();
    }
    public int QuoteId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<Product>? Products { get; }
    public ICollection<QuoteProducts>? QuotesProducts { get; }
}
