using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServiceQuotes.DTOs.Quote;

public class QuoteProductsRequestDTO
{
    [JsonIgnore]
    public int QuoteId { get; set; }
    [Required]
    public Guid ProductId { get; set; }
    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; }
    [Required]
    [Range(1, 1000000)]
    public decimal Price { get; set; }
}
