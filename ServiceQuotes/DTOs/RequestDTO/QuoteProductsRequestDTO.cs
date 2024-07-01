using System.Text.Json.Serialization;

namespace ServiceQuotes.DTOs.RequestDTO;

public class QuoteProductsRequestDTO
{
    [JsonIgnore]
    public int QuoteId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
