using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceQuotes.Models;

public class QuoteProducts
{
    public Guid ProductId { get; set; }
    public int QuoteId { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
}
