using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceQuotes.Models;

public class Quote
{
    public Quote()
    {
        Products = new Collection<Product>();
        QuotesProducts = new Collection<QuoteProducts>();
    }
    [Key]
    public int QuoteId { get; set; }
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; set; }
    [Required]
    [StringLength(200)]
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<Product>? Products { get; }
    public ICollection<QuoteProducts>? QuotesProducts { get; }
}
