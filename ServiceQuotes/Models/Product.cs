using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.Models;

public class Product
{
    public Product()
    {
        Quotes = new Collection<Quote>();
        QuotesProducts = new Collection<QuoteProducts>();
    }
    [Key]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(200)]
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Quote> Quotes { get; }
    public ICollection<QuoteProducts>? QuotesProducts { get; }
}
