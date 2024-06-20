namespace ServiceQuotes.Models;

public class QuoteProducts
{
    public Guid ProductId { get; set; }
    public int QuoteId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
