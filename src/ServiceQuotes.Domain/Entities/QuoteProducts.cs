using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceQuotes.Domain.Entities;

public class QuoteProducts
{
    public Guid ProductId { get; set; }

    public int QuoteId { get; set; }

    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, 10000.00)]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
}
