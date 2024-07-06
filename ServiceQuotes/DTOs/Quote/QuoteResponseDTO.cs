namespace ServiceQuotes.DTOs.Quote;

public class QuoteResponseDTO
{
    public int QuoteId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
