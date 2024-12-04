namespace ServiceQuotes.Application.DTO.Quote;
public class QuoteResumedDTO
{
    public int QuoteId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? CustomerName { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}
