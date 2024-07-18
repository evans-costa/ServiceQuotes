using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.Domain.Entities;

public sealed class Product
{
    public Product()
    {
        Quotes = new Collection<Quote>();
        QuoteProducts = new Collection<QuoteProducts>();
    }

    [Key]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string? Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ICollection<Quote> Quotes { get; }
    public ICollection<QuoteProducts> QuoteProducts { get; }
}
