using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServiceQuotes.Models;

public class Product
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
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [JsonIgnore]
    public ICollection<Quote> Quotes { get; }
    public ICollection<QuoteProducts> QuoteProducts { get; }
}
