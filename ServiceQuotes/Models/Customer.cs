using System.Collections.ObjectModel;

namespace ServiceQuotes.Models;

public class Customer
{
    public Customer()
    {
        Quotes = new Collection<Quote>();
    }

    public Guid CustomerId { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Quote>? Quotes { get; }
}
